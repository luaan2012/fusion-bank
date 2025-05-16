using fusion.bank.core.Messages.Requests;
using fusion.bank.deposit.services;
using MassTransit;

namespace fusion.bank.creditcard.api.Configuration
{
    public static class MassTransitConfig
    {
        public static void AddMassTransitConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(busCfg =>
            {
                busCfg.SetKebabCaseEndpointNameFormatter();

                busCfg.AddConsumer<DepositedAccountResponse>();

                busCfg.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(configuration.GetConnectionString("RabbitMQ")));

                    cfg.ConfigureEndpoints(ctx);
                });
            });
        }
    }
}
