﻿using System;

namespace Hedger.Common.Domain.Buckets
{
    public class Bucket
    {
        public string AssetId { get; }

        public string OtherAssetId { get { return AssetId == BaseAssetId ? QuoteAssetId : BaseAssetId; } }

        public string AssetPairId { get; }

        public string BaseAssetId { get; }

        public string QuoteAssetId { get; }

        public decimal BaseVolume { get; private set; }

        public decimal QuoteVolume { get; private set; }

        public decimal AveragePrice { get; private set; }

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

            UpdateAveragePrice();
        }

        public void AddQuoteVolume(decimal volume)
        {
            QuoteVolume += volume;

            UpdateAveragePrice();
        }

        private void UpdateAveragePrice()
        {
            AveragePrice = Math.Abs(QuoteVolume / BaseVolume);
        }
    }
}
