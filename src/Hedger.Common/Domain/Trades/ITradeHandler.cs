using System.Threading.Tasks;

namespace Hedger.Common.Domain.Trades
{
    public interface ITradeHandler
    {
        Task HandleAsync(Trade trade);
    }
}
