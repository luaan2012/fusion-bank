using fusion.bank.account.service;
using MassTransit;

namespace fusion.bank.events.api.Configuration
{
    public static class MassTransitConfig
    {
        public static void AddMassTransitConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(busCfg =>
            {
                busCfg.SetKebabCaseEndpointNameFormatter();

                busCfg.AddConsumer<EventConsumer>();


                busCfg.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(configuration.GetConnectionString("RabbitMQ")));

                    cfg.ConfigureEndpoints(ctx);
                });
            });
        }
    }
}
