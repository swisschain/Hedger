using System;

namespace Hedger.Common.Domain.Buckets
{
    public class BucketUpdate
    {
        public string AssetPairId { get; }

        public string BaseAssetId { get; }

        public string QuoteAssetId { get; }

        public decimal BaseVolume { get; }

        public decimal QuoteVolume { get; }

        public decimal AveragePrice { get; }

        public BucketUpdate(string assetPairId, string baseAssetId, string quoteAssetId, decimal baseVolume, decimal quoteVolume)
        {
            AssetPairId = assetPairId;
            BaseAssetId = baseAssetId;
            QuoteAssetId = quoteAssetId;
            BaseVolume = baseVolume;
            QuoteVolume = quoteVolume;
            AveragePrice = Math.Abs(QuoteVolume / BaseVolume);
        }
    }
}
