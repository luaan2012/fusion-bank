using fusion.bank.account.service;
using fusion.bank.core.Messages.Requests;
using MassTransit;

namespace fusion.bank.account.api.Configuration
{
    public static class MassTransitConfig
    {
        public static void AddMassTransitConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(busCfg =>
            {
                busCfg.SetKebabCaseEndpointNameFormatter();

                busCfg.AddConsumer<CreatedAccountConsumer>();
                busCfg.AddConsumer<NewDepositAccountConsumer>();
                busCfg.AddConsumer<NewTransferAccountConsumer>();
                busCfg.AddConsumer<NewCreditCardccountConsumer>();
                busCfg.AddConsumer<NewCreditCardCreatedAccountConsumer>();
                busCfg.AddConsumer<NewInvestmentRequestConsumer>();
                busCfg.AddConsumer<NewInvestmentRequestPutConsumer>();

                busCfg.AddRequestClient<NewDepositCentralRequest>();
                busCfg.AddRequestClient<NewTransferCentralRequest>();
                busCfg.AddRequestClient<NewKeyAccountRequest>();

                busCfg.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(configuration.GetConnectionString("RabbitMQ")));

                    cfg.ConfigureEndpoints(ctx);
                });
            });
        }
    }
}
