using fusion.bank.core.Interfaces;
using fusion.bank.core.services;
using fusion.bank.transfer.domain.Interfaces;
using fusion.bank.transfer.repository;
using fusion.bank.transfer.services;

namespace fusion.bank.central.api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<ITransferRepository, TransferRepository>();

            services.AddHostedService<TransferBackground>();

            services.AddHostedService<TransferProcessingService>();

            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }
    }
}
