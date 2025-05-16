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

                busCfg.AddConsumer<CreditCardCreatedAccountConsumer>();
                busCfg.AddConsumer<DepositAccountConsumer>();
                busCfg.AddConsumer<TransferAccountConsumer>();
                busCfg.AddConsumer<CreditCardAccountConsumer>();
                busCfg.AddConsumer<CreditCardCreatedAccountConsumer>();
                busCfg.AddConsumer<InvestmentRequestConsumer>();
                busCfg.AddConsumer<InvestmentRequestPutConsumer>();
                busCfg.AddConsumer<TransactionConsumer>();

                busCfg.AddRequestClient<NewDepositCentralRequest>();
                busCfg.AddRequestClient<TransactionCentralRequest>();
                busCfg.AddRequestClient<NewKeyAccountRequest>();
                busCfg.AddRequestClient<CreditCardTransactionRequest>();

                busCfg.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(configuration.GetConnectionString("RabbitMQ")));

                    cfg.ConfigureEndpoints(ctx);
                });
            });
        }
    }
}
