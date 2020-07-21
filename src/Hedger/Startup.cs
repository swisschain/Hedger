using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hedger.Common.Configuration;
using Hedger.Common.Services;
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

            services
                //.AddAutoMapper(typeof(AutoMapperProfile), typeof(Domain.Persistence.AutoMapperProfile))
                .AddControllersWithViews();

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

            services.AddGrpcReflection();
        }

        protected override void ConfigureExt(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthorization();

            //app.ApplicationServices.GetRequiredService<AutoMapper.IConfigurationProvider>()
            //    .AssertConfigurationIsValid();

            //app.ApplicationServices.GetRequiredService<ConnectionFactory>()
            //    .EnsureMigration();
        }

        protected override void ConfigureContainerExt(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModule(Config));
        }

        protected override void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            base.RegisterEndpoints(endpoints);

            endpoints.MapGrpcReflectionService();

            endpoints.MapGrpcService<MonitoringService>();
        }
    }
}
