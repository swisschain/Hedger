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
            lock (_sync)
            {
                // 1. if there is only one bucket with both assets then update it
                Bucket baseBucket;
                Bucket quoteBucket;

                _buckets.TryGetValue(trade.BaseAssetId, out baseBucket);
                _buckets.TryGetValue(trade.QuoteAssetId, out quoteBucket);

                if (baseBucket != null)
                {
                    if (baseBucket.OtherAssetId == trade.QuoteAssetId)
                    {
                        baseBucket.AddBaseVolume(trade.BaseVolume);
                        baseBucket.AddQuoteVolume(trade.QuoteVolume);

                        return;
                    }

                    // update 
                    baseBucket.AddBaseVolume(trade.BaseVolume);

                    if (quoteBucket == null)
                        throw new InvalidOperationException("'Quote' bucket is null.");

                    var conversionPrice = GetPrice(trade.QuoteAssetId, baseBucket.OtherAssetId);

                    var bucketQuoteVolume = trade.QuoteVolume* conversionPrice;

                    baseBucket.AddQuoteVolume(bucketQuoteVolume);
                }

                if (quoteBucket != null)
                {
                    if (quoteBucket.OtherAssetId == trade.BaseAssetId)
                    {
                        quoteBucket.AddBaseVolume(trade.BaseVolume);
                        quoteBucket.AddQuoteVolume(trade.QuoteVolume);

                        return;
                    }

                    if (baseBucket == null)
                        throw new InvalidOperationException("'Base' bucket is null.");

                    // continue mirroring previous method
                }

                throw new InvalidOperationException("Both 'Base' and 'Quote' buckets are null.");
            }
        }

        private decimal GetPrice(string baseAssetId, string quoteAssetId)
        {
            throw new NotImplementedException();
        }
    }
}
