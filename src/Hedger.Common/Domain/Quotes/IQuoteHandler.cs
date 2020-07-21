using System.Threading.Tasks;

namespace Hedger.Common.Domain.Quotes
{
    public interface IQuoteHandler
    {
        Task HandleAsync(Quote quote);
    }
}
