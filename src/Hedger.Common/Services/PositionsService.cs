using System;
using System.Collections.Generic;
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
            // todo: validate that assetId is in bucket.AssetPairId

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
            var baseAssetId = trade.BaseAssetId;
            var quoteAssetId = trade.QuoteAssetId;

            lock (_sync)
            {
                Bucket baseBucket;
                Bucket quoteBucket;

                _buckets.TryGetValue(baseAssetId, out baseBucket);
                _buckets.TryGetValue(quoteAssetId, out quoteBucket);

                if (baseBucket == null)
                {
                    // todo: create a bucket for it? or make sure that it won't happen in the first place?
                    throw new InvalidOperationException($"Can't find the bucket for '{baseAssetId}'.");
                }

                if (quoteBucket == null)
                {
                    // todo: create a bucket for it? or make sure that it won't happen in the first place?
                    throw new InvalidOperationException($"Can't find the bucket for '{quoteAssetId}'.");
                }

                // 1. if there is a bucket with both assets then update it
                if (baseBucket.OtherAssetId == quoteAssetId)
                {
                    baseBucket.AddBaseVolume(trade.BaseVolume);
                    baseBucket.AddQuoteVolume(trade.QuoteVolume);
                }

                if (quoteBucket.OtherAssetId == baseAssetId)
                {
                    baseBucket.AddBaseVolume(trade.QuoteVolume);
                    baseBucket.AddQuoteVolume(trade.BaseVolume);
                }


            }
        }
    }
}
