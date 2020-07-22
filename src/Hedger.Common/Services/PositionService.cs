using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hedger.Common.Domain.Buckets;
using Hedger.Common.Domain.Quotes;
using Hedger.Common.Domain.Trades;
using Microsoft.Extensions.Logging;

namespace Hedger.Common.Services
{
    public class PositionService : ITradeHandler
    {
        private readonly object _sync = new object();
        private readonly Dictionary<string, Bucket> _buckets = new Dictionary<string, Bucket>();
        private readonly InternalQuotesService _internalQuotesService;
        private readonly ILogger<PositionService> _logger;

        public PositionService(InternalQuotesService internalQuotesService, ILogger<PositionService> logger)
        {
            _internalQuotesService = internalQuotesService;
            _logger = logger;
        }

        public void AddBucket(string assetId, Bucket bucket)
        {
            // todo: validate that assetId is in the bucket.AssetPairId

            // todo: validate that there is no bucket with both assets

            // todo: how and when it has to be initialized with all the buckets?

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

            var newBucketUpdate = new BucketUpdate(
                trade.AssetPairId,
                trade.BaseAssetId,
                trade.QuoteAssetId,
                trade.BaseVolume,
                trade.QuoteVolume);

            bucketUpdates.Add(newBucketUpdate);

            var allBuckets = new List<Bucket>();

            lock (_sync)
            {
                allBuckets.AddRange(
                    _buckets.Values.ToList());
            }

            BucketUpdate bucketUpdate;

            do
            {
                bucketUpdate = BucketUpdateService.GetUpdateWithNoBucket(bucketUpdates, allBuckets);

                if (bucketUpdate != null)
                {
                    bucketUpdates.Remove(bucketUpdate);

                    // 1. BaseBucket

                    Bucket baseBucket;  // BTCUSD

                    lock (_sync)
                    {
                        _buckets.TryGetValue(bucketUpdate.BaseAssetId, out baseBucket);
                    }

                    if (baseBucket == null)
                        throw new InvalidOperationException($"Couldn't find the bucket for '{bucketUpdate.BaseAssetId}'");

                    //             EUR          !=              USD
                    if (baseBucket.QuoteAssetId == bucketUpdate.QuoteAssetId)
                    {
                        var baseBucketUpdate = new BucketUpdate(
                            baseBucket.AssetPairId,
                            baseBucket.BaseAssetId,
                            baseBucket.QuoteAssetId,
                            trade.BaseVolume,
                            trade.QuoteVolume);

                        bucketUpdates.Add(baseBucketUpdate);
                    }
                    else
                    {
                        //                                                          EUR                 USD
                        var crossQuote = _internalQuotesService.GetQuote(baseBucket.QuoteAssetId, trade.QuoteAssetId);

                        // todo: double check that it's correct
                        var crossRate = trade.QuoteVolume > 0 ? crossQuote.Ask : crossQuote.Bid;

                        var oppositeVolume = -(trade.QuoteVolume * crossRate);

                        var baseBucketUpdate = new BucketUpdate(
                            crossQuote.AssetPairId,
                            crossQuote.BaseAssetId,
                            crossQuote.QuoteAssetId,
                            trade.BaseVolume,
                            oppositeVolume);

                        bucketUpdates.Add(baseBucketUpdate);
                    }

                    // 2. QuoteBucket

                    Bucket quoteBucket; // EURUSD

                    lock (_sync)
                    {
                        _buckets.TryGetValue(bucketUpdate.QuoteAssetId, out quoteBucket);
                    }

                    if (quoteBucket == null)
                        throw new InvalidOperationException($"Couldn't find the bucket for '{bucketUpdate.QuoteAssetId}'");

                    //              USD                          USD
                    if (quoteBucket.QuoteAssetId == bucketUpdate.QuoteAssetId)
                    {
                        //                                                           EUR                      USD
                        var crossQuote = _internalQuotesService.GetQuote(quoteBucket.BaseAssetId, quoteBucket.QuoteAssetId);

                        // todo: double check that it's correct
                        var crossRate = trade.BaseVolume > 0 ? crossQuote.Ask : crossQuote.Bid;

                        // todo: double check that it's correct
                        var oppositeVolume = -(trade.BaseVolume * crossRate);

                        var baseBucketUpdate = new BucketUpdate(
                            quoteBucket.AssetPairId,
                            quoteBucket.BaseAssetId,
                            quoteBucket.QuoteAssetId,
                            trade.QuoteVolume,
                            oppositeVolume);

                        bucketUpdates.Add(baseBucketUpdate);
                    }
                    else
                    {
                        ////                                                          EUR                 USD
                        //var crossQuote = _internalQuotesService.GetQuote(baseBucket.QuoteAssetId, trade.QuoteAssetId);

                        //// todo: double check that it's correct
                        //var crossRate = trade.QuoteVolume > 0 ? crossQuote.Ask : crossQuote.Bid;

                        //var oppositeVolume = -(trade.QuoteVolume * crossRate);

                        //var baseBucketUpdate = new BucketUpdate(
                        //    crossQuote.AssetPairId,
                        //    crossQuote.BaseAssetId,
                        //    crossQuote.QuoteAssetId,
                        //    trade.BaseVolume,
                        //    oppositeVolume);

                        //bucketUpdates.Add(baseBucketUpdate);
                    }

                }
            } while (bucketUpdate != null);

            // log 

            // update buckets
        }
    }
}
