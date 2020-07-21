using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Lykke.HftApi.ApiContract;

namespace Hedger.InternalExchangeClient
{
    public class LykkeExchangeClient
    {
        protected GrpcChannel Channel { get; }

        public PublicService.PublicServiceClient PublicApi { get; }

        public PrivateService.PrivateServiceClient PrivateApi { get; }

        public LykkeExchangeClient(string url, string apiKey)
        {
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (!string.IsNullOrEmpty(apiKey))
                {
                    metadata.Add("Authorization", $"Bearer {apiKey}");
                }
                return Task.CompletedTask;
            });

            Channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            });

            Channel = GrpcChannel.ForAddress(url);

            PublicApi = new PublicService.PublicServiceClient(Channel);

            PrivateApi = new PrivateService.PrivateServiceClient(Channel);
        }
    }
}
