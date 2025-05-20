using fusion.bank.core.Interfaces;
using fusion.bank.core.services;
using fusion.bank.core.Services;
using fusion.bank.deposit.domain.Interfaces;
using fusion.deposit.deposit.repository;

namespace fusion.bank.creditcard.api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IDepositRepository, DepositRepository>();

            services.AddHostedService<ProcessingBackGroundService>();

            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }
    }
}
