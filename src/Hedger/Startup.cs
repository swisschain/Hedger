using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hedger.Common.Configuration;
using Hedger.Common.HostedServices;
using Hedger.GrpcServices;
using Swisschain.Sdk.Server.Common;

namespace Hedger
{
    public sealed class Startup : SwisschainStartup<AppConfig>
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }

        protected override void ConfigureServicesExt(IServiceCollection services)
        {
            base.ConfigureServicesExt(services);

            //services.AddMassTransit(x =>
            //{
            //    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            //    {
            //        cfg.Host(Config.RabbitMq.HostUrl,
            //            host =>
            //            {
            //                host.Username(Config.RabbitMq.Username);
            //                host.Password(Config.RabbitMq.Password);
            //            });

            //        cfg.SetLoggerFactory(provider.GetRequiredService<ILoggerFactory>());
            //    }));
            //});

            //services.AddHostedService<BusHost>();
        }

        protected override void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            base.RegisterEndpoints(endpoints);

            endpoints.MapGrpcService<MonitoringService>();
        }
    }
}
