using Autofac;
using Hedger.Common.Configuration;
using Hedger.Common.Domain.Quotes;
using Hedger.InternalExchangeClient;
using Microsoft.Extensions.Hosting;

namespace Hedger.Common.Services
{
    public class AutofacModule : Module
    {
        private readonly AppConfig _config;

        public AutofacModule(AppConfig config)
        {
            _config = config;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Domain.Quotes.InternalQuotesService>()
                .AsSelf()
                .As<IQuoteHandler>()
                .SingleInstance();

            builder.RegisterType<LykkeExchangeClient>()
                .AsSelf()
                .WithParameter("url", _config.LykkeHftClient.Url)
                .WithParameter("apiKey", _config.LykkeHftClient.ApiKey)
                .SingleInstance();

            builder.RegisterType<InternalQuotesMediator>()
                .AsSelf()
                .As<IHostedService>()
                .SingleInstance();
        }
    }
}
