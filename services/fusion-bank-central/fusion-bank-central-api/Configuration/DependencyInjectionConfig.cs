using fusion.bank.central.domain.Interfaces;
using fusion.bank.central.repository;

namespace fusion.bank.central.api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IBankRepository, BankRepository>();
        }
    }
}
