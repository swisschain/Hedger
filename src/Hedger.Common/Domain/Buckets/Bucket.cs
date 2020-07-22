namespace Hedger.Common.Domain.Buckets
{
    public class Bucket
    {
        public string AssetId { get; }

        public bool IsStraight { get; }

        public string AssetPairId { get; }

        public string BaseAssetId { get; }

        public string QuoteAssetId { get; }

        public decimal BaseVolume { get; private set; }

        public decimal QuoteVolume { get; private set; }

        public Bucket(string assetId, string assetPairId, string baseAssetId, string quoteAssetId)
        {
            AssetId = assetId;
            AssetPairId = assetPairId;
            BaseAssetId = baseAssetId;
            QuoteAssetId = quoteAssetId;
        }

        public void AddBaseVolume(decimal volume)
        {
            BaseVolume += volume;
        }

        public void AddQuoteVolume(decimal volume)
        {
            QuoteVolume += volume;
        }
    }
}
