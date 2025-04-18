using fusion.bank.central.service;
using MassTransit;

namespace fusion.bank.central.api.Configuration
{
    public static class MassTransitConfig
    {
        public static void AddMassTransitConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(busCfg =>
            {
                busCfg.SetKebabCaseEndpointNameFormatter();

                busCfg.AddConsumer<NewAccountCentralConsumer>();
                busCfg.AddConsumer<NewDepositCentralConsumer>();
                busCfg.AddConsumer<NewTransferCentralConsumer>();
                busCfg.AddConsumer<NewKeyAccountCentralConsumer>();

                busCfg.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(configuration.GetConnectionString("RabbitMQ")));

                    cfg.ConfigureEndpoints(ctx);
                });
            });
        }
    }
}
