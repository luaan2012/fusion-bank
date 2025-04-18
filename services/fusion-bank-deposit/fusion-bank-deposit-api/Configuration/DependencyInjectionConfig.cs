using fusion.bank.deposit.domain.Interfaces;
using fusion.deposit.deposit.repository;

namespace fusion.bank.creditcard.api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IDepositRepository, DepositRepository>();
        }
    }
}
