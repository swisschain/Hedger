using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hedger.Common.Domain.Quotes;
using Hedger.InternalExchangeClient;
using Lykke.HftApi.ApiContract;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hedger.Common.Services
{
    public class InternalQuotesMediator: IHostedService
    {
        private const string LykkeExchange = "Lykke";

        private readonly LykkeExchangeClient _client;
        private readonly IQuoteHandler[] _handlers;
        private readonly ILogger<InternalQuotesMediator> _logger;

        public InternalQuotesMediator(
            LykkeExchangeClient client,
            IQuoteHandler[] handlers,
            ILogger<InternalQuotesMediator> logger)
        {
            _client = client;
            _handlers = handlers;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Initializing internal quotes...");

            var priceRequest = new PricesRequest();

            _logger.LogInformation("Receiving all internal quotes...");

            var pricesResponse = await _client.PublicApi.GetPricesAsync(priceRequest);
            
            ValidateResponse(pricesResponse.Error);

            _logger.LogInformation($"Received {pricesResponse.Payload.Count} internal quotes.");

            foreach (var priceUpdate in pricesResponse.Payload)
            {
                var quote = Map(priceUpdate);

                foreach (var quoteHandler in _handlers)
                {
                    await quoteHandler.HandleAsync(quote);
                }
            }

            _logger.LogInformation("Handled all internal quotes.");

            // todo: to make it consistent we need to subscribe to updates first,
            // todo: then receive all quotes and update only those that has not been received yet
            SubscribeAndHandleQuoteUpdatesAsync();
        }

        private async Task SubscribeAndHandleQuoteUpdatesAsync()
        {
            _logger.LogInformation("Subscribing to all internal quotes updates...");

            var priceUpdateRequest = new PriceUpdatesRequest();

            var priceStream = _client.PublicApi.GetPriceUpdates(priceUpdateRequest);

            _logger.LogInformation("Subscribed to all internal quotes updates.");

            var cancellationToken = new CancellationToken();

            // todo: implement reconnection in case of disconnect
            while (await priceStream.ResponseStream.MoveNext(cancellationToken))
            {
                var priceUpdate = priceStream.ResponseStream.Current;

                var quote = Map(priceUpdate);

                foreach (var quoteHandler in _handlers)
                {
                    await quoteHandler.HandleAsync(quote);
                }
            }
        }

        void ValidateResponse(Error error)
        {
            if (error != null && error.Code != ErrorCode.Success)
            {
                _logger.LogError("Error while initializing quotes from internal exchange. {@context}.",
                    new { error.Code, error.Message, error.Fields });

                throw new IOException("Error while initializing quotes from internal exchange.");
            }
        }

        private static Quote Map(PriceUpdate priceUpdate)
        {
            var quote = new Quote(
                priceUpdate.AssetPairId,
                priceUpdate.Timestamp.ToDateTime(),
                decimal.Parse(priceUpdate.Ask, CultureInfo.InvariantCulture),
                decimal.Parse(priceUpdate.Bid, CultureInfo.InvariantCulture),
                LykkeExchange);

            return quote;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //todo: implement unsubscribing from quotes updates
            return Task.CompletedTask;
        }
    }
}
