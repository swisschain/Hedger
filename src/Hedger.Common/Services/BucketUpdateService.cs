using System.Collections.Generic;
using Hedger.Common.Domain.Buckets;

namespace Hedger.Common.Services
{
    public class BucketUpdateService
    {
        public static BucketUpdate GetUpdateWithNoBucket(
            IReadOnlyCollection<BucketUpdate> bucketUpdates,
            IReadOnlyCollection<Bucket> allBuckets)
        {
            // todo: optimize this search with dictionaries
            foreach (var bucketUpdate in bucketUpdates)
            {
                foreach (var bucket in allBuckets)
                {
                    var isStraightFound = bucketUpdate.BaseAssetId == bucket.BaseAssetId
                                  && bucketUpdate.QuoteAssetId == bucket.QuoteAssetId;

                    if (isStraightFound)
                        continue;

                    var isReversedFound = bucketUpdate.BaseAssetId == bucket.QuoteAssetId
                                   && bucketUpdate.QuoteAssetId == bucket.BaseAssetId;

                    if (isReversedFound)
                        continue;

                    return bucketUpdate;
                }
            }

            return null;
        }
    }
}
