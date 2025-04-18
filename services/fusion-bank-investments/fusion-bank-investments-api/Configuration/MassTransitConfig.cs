using fusion.bank.core.Messages.Requests;
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

                busCfg.AddRequestClient<NewAccountRequestInformation>();
                busCfg.AddRequestClient<NewInvestmentRequest>();
                busCfg.AddRequestClient<NewAccountRequestPutAmount>();

                busCfg.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(configuration.GetConnectionString("RabbitMQ")));

                    cfg.ConfigureEndpoints(ctx);
                });
            });
        }
    }
}
