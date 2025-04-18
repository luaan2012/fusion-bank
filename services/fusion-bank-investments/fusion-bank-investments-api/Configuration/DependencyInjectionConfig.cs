using fusion.bank.investments.domain.Interfaces;
using fusion.bank.investments.repository;

namespace fusion.bank.creditcard.api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IInvestmentRepository, InvestmentRepository>();
        }
    }
}
