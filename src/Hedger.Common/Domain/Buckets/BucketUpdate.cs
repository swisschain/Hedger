namespace Hedger.Common.Domain.Buckets
{
    public class BucketUpdate
    {
        public string AssetPairId { get; set; }

        public string BaseAssetId { get; set; }

        public string QuoteAssetId { get; set; }

        public decimal Volume { get; set; }

        public decimal OppositeVolume { get; set; }

        public BucketUpdate(string assetPairId, string baseAssetId, string quoteAssetId, decimal volume, decimal oppositeVolume)
        {
            AssetPairId = assetPairId;
            BaseAssetId = baseAssetId;
            QuoteAssetId = quoteAssetId;
            Volume = volume;
            OppositeVolume = oppositeVolume;
        }
    }
}
