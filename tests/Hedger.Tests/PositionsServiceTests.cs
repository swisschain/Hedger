using System;
using System.Threading.Tasks;
using Hedger.Common.Domain.Buckets;
using Hedger.Common.Domain.Quotes;
using Hedger.Common.Domain.Trades;
using Hedger.Common.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Hedger.Tests
{
    public class PositionsServiceTests
    {
        [Fact(Skip = "not ready yet")]
        public async Task Simple1()
        {
            // arrange

            // Buckets:
            // BTC/USD

            // Prices:
            // BTC/USD

            var btcUsdBucket = new Bucket("BTC", "BTCUSD", "BTC", "USD");

            var internalQuotesService = new InternalQuotesService(NullLogger<InternalQuotesService>.Instance);
            var positionService = new PositionsService(internalQuotesService, NullLogger<PositionsService>.Instance);

            var bucket = new Bucket("BTC", "BTCUSD", "BTC", "USD");
            positionService.AddBucket("BTC", bucket);

            var trade = new Trade("1", "1", "BTCUSD", "BTC", "USD", 1, -10000, 10000, DateTime.Now);

            // act

            await positionService.HandleAsync(trade);

            // assert

            var updatedBucket = positionService.GetBucket("BTC");

            Assert.Equal(1, updatedBucket.BaseVolume);
            Assert.Equal(-10000, updatedBucket.QuoteVolume);
        }

        [Fact(Skip = "not ready yet")]
        public async Task Simple2()
        {
            // arrange

            // Buckets:
            // BTC/USD

            // Trade:
            // USD/BTC
        }
    }
}
