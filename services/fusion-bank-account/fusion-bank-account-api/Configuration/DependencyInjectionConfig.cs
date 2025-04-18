using fusion.bank.account.domain.Interfaces;
using fusion.bank.account.repository;

namespace fusion.bank.account.api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
        }
    }
}
