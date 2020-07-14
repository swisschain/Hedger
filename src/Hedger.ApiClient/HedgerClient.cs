using Swisschain.Hedger.Hedger.ApiClient.Common;
using Swisschain.Hedger.Hedger.ApiContract;

namespace Swisschain.Hedger.Hedger.ApiClient
{
    public class HedgerClient : BaseGrpcClient, IHedgerClient
    {
        public HedgerClient(string serverGrpcUrl) : base(serverGrpcUrl)
        {
            Monitoring = new Monitoring.MonitoringClient(Channel);
        }

        public Monitoring.MonitoringClient Monitoring { get; }
    }
}
