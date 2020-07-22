using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hedger.Common.Domain.Buckets;
using Hedger.Common.Domain.Quotes;
using Hedger.Common.Domain.Trades;
using Microsoft.Extensions.Logging;

namespace Hedger.Common.Services
{
    public class PositionsService : ITradeHandler
    {
        private readonly object _sync = new object();
        private readonly Dictionary<string, Bucket> _buckets = new Dictionary<string, Bucket>();
        private readonly InternalQuotesService _internalQuotesService;
        private readonly ILogger<PositionsService> _logger;

        public PositionsService(InternalQuotesService internalQuotesService, ILogger<PositionsService> logger)
        {
            _internalQuotesService = internalQuotesService;
            _logger = logger;
        }

        public void AddBucket(string assetId, Bucket bucket)
        {
            // todo: validate that assetId is in the bucket.AssetPairId

            // todo: validate that there is no bucket with both assets

            lock (_sync)
            {
                _buckets[assetId] = bucket;
            }
        }

        public Bucket GetBucket(string assetId)
        {
            lock (_sync)
            {
                Bucket bucket;

                _buckets.TryGetValue(assetId, out bucket);

                return bucket;
            }
        }

        public async Task HandleAsync(Trade trade)
        {
            var bucketUpdates = new List<BucketUpdate>();

            var bucketUpdate = new BucketUpdate(trade.AssetPairId, trade.BaseAssetId, trade.QuoteAssetId, trade.BaseVolume, trade.QuoteVolume);
            bucketUpdates.Add(bucketUpdate);

            var allBuckets = new List<Bucket>();

            lock (_sync)
            {
                allBuckets.AddRange(
                    _buckets.Values.ToList());
            }

            BucketUpdateService.GetUpdateWithoutBucket(bucketUpdates, allBuckets);
        }
    }
}
