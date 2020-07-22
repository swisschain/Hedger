using System.Collections.Generic;
using Hedger.Common.Domain.Buckets;

namespace Hedger.Common.Services
{
    public class BucketUpdateService
    {
        public static BucketUpdate GetUpdateWithoutBucket(
            IReadOnlyCollection<BucketUpdate> bucketUpdates,
            IReadOnlyCollection<Bucket> allBuckets)
        {
            // todo: optimize
            foreach (var bucketUpdate in bucketUpdates)
            {
                foreach (var bucket in allBuckets)
                {
                    var isStraight = bucketUpdate.BaseAssetId == bucket.BaseAssetId
                                  && bucketUpdate.QuoteAssetId == bucket.QuoteAssetId;

                    if (isStraight)
                        continue;

                    var isReversed = bucketUpdate.BaseAssetId == bucket.QuoteAssetId
                                   && bucketUpdate.QuoteAssetId == bucket.BaseAssetId;

                    if (isReversed)
                        continue;

                    return bucketUpdate;
                }
            }

            return null;
        }
    }
}
