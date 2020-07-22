using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hedger.Common.Domain.Trades;
using Hedger.InternalExchangeClient;
using Lykke.HftApi.ApiContract;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Trade = Hedger.Common.Domain.Trades.Trade;
using LykkeTrade = Lykke.HftApi.ApiContract.Trade;

namespace Hedger.Common.Services
{
    public class InternalTradesService: IHostedService
    {
        private const string LykkeExchange = "Lykke";

        private readonly LykkeExchangeClient _client;
        private readonly ITradeHandler[] _handlers;
        private readonly ILogger<InternalTradesService> _logger;

        public InternalTradesService(
            LykkeExchangeClient client,
            ITradeHandler[] handlers,
            ILogger<InternalTradesService> logger)
        {
            _client = client;
            _handlers = handlers;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Initializing internal trades...");

            var tradesRequest = new TradesRequest();

            _logger.LogInformation("Receiving all internal quotes...");

            //tradesRequest.From

            //var pricesResponse = await _client.PrivateApi.Get(priceRequest);
            
            //ValidateResponse(pricesResponse.Error);

            //_logger.LogInformation($"Received {pricesResponse.Payload.Count} internal quotes.");

            //foreach (var priceUpdate in pricesResponse.Payload)
            //{
            //    var quote = Map(priceUpdate);

            //    foreach (var quoteHandler in _handlers)
            //    {
            //        await quoteHandler.HandleAsync(quote);
            //    }
            //}

            //_logger.LogInformation("Handled all internal quotes.");
        }

        void ValidateResponse(Error error)
        {
            if (error != null && error.Code != ErrorCode.Success)
            {
                _logger.LogError("Error while initializing trades from internal exchange. {@context}.",
                    new { error.Code, error.Message, error.Fields });

                throw new IOException("Error while initializing trades from internal exchange.");
            }
        }

        private static Trade Map(LykkeTrade lykkeTrade)
        {
            var trade = new Trade(
                lykkeTrade.Id,
                lykkeTrade.OrderId,
                lykkeTrade.AssetPairId,
                lykkeTrade.BaseAssetId,
                lykkeTrade.QuoteAssetId,
                decimal.Parse(lykkeTrade.BaseVolume),
                decimal.Parse(lykkeTrade.QuoteVolume),
                decimal.Parse(lykkeTrade.Price),
                lykkeTrade.Timestamp.ToDateTime());

            return trade;
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
