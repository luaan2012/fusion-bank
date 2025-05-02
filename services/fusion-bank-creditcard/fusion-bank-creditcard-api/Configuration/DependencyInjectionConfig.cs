using fusion.bank.central.repository;
using fusion.bank.core.Interfaces;
using fusion.bank.core.services;
using fusion.bank.creditcard.domain.Interfaces;
using fusion.bank.creditcard.services;

namespace fusion.bank.creditcard.api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<ICreditCartRepository, CreditCardRepository>();

            services.AddScoped<IGenerateCreditCardService, GenerateCreditCardService>();

            services.AddHostedService<CreditCardProcessingService>();

            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }
    }
}
