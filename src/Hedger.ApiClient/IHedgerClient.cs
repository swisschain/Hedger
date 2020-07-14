using Swisschain.Hedger.Hedger.ApiContract;

namespace Swisschain.Hedger.Hedger.ApiClient
{
    public interface IHedgerClient
    {
        Monitoring.MonitoringClient Monitoring { get; }
    }
}
