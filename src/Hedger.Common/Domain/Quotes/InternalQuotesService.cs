using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Hedger.Common.Domain.Quotes
{
    // todo: asset pair service to 
    public class InternalQuotesService : IQuoteHandler
    {
        private readonly ConcurrentDictionary<string, Quote> _cache = new ConcurrentDictionary<string, Quote>();
        private readonly ILogger<InternalQuotesService> _logger;

        public InternalQuotesService(ILogger<InternalQuotesService> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(Quote quote)
        {
            // todo: check that timestamp is later then existed
            _cache[quote.AssetPair] = quote;
        }

        public async Task<IReadOnlyCollection<Quote>> GetAllAsync()
        {
            return _cache.Values.ToList();
        }

        public async Task<Quote> GetAsync(string assetPair)
        {
            var isFound = _cache.TryGetValue(assetPair, out var quote);

            if (isFound)
                return quote;

            return null;
        }
    }
}
