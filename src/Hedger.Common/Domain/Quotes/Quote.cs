using System;

namespace Hedger.Common.Domain.Quotes
{
    public class Quote
    {
        public string Source { get; }
        public string AssetPairId { get; }
        // todo: has to be filled in constructor
        public string BaseAssetId { get; }
        public string QuoteAssetId { get; }
        public decimal Ask { get; }
        public decimal Bid { get; }
        public DateTime Timestamp { get; }
        public decimal Mid { get; }
        public decimal Spread { get; }

        public Quote(string assetPairId, DateTime timestamp, decimal ask, decimal bid, string source)
        {
            AssetPairId = assetPairId;
            Timestamp = timestamp;
            Ask = ask;
            Bid = bid;
            Mid = (ask + bid) / 2m;
            Spread = Ask - Bid;
            Source = source;
        }
    }
}
