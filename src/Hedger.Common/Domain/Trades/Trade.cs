using System;

namespace Hedger.Common.Domain.Trades
{
    public class Trade
    {
        public string Id { get; }

        public string OrderId { get; }

        public string AssetPairId { get; }

        public string BaseAssetId { get; }

        public string QuoteAssetId { get; }

        public decimal BaseVolume { get; }

        public decimal QuoteVolume { get; }

        public decimal Price { get; }

        public DateTime Timestamp { get; }

        public Trade(string id, string orderId, string assetPairId, string baseAssetId, string quoteAssetId, decimal baseVolume, decimal quoteVolume, decimal price, DateTime timestamp)
        {
            Id = id;
            OrderId = orderId;
            AssetPairId = assetPairId;
            BaseAssetId = baseAssetId;
            QuoteAssetId = quoteAssetId;
            BaseVolume = baseVolume;
            QuoteVolume = quoteVolume;
            Price = price;
            Timestamp = timestamp;
        }
    }
}
