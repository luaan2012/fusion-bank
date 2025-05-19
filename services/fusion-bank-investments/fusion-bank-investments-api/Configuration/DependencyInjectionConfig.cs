using fusion.bank.core.Interfaces;
using fusion.bank.core.services;
using fusion.bank.investments.domain.Interfaces;
using fusion.bank.investments.repository;
using fusion.bank.investments.Services;
using StackExchange.Redis;

namespace fusion.bank.investments.api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var connection = ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisCache"));
                return connection;
            });

            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddHttpClient<IInvestmentService, InvestmentService>();
            services.AddScoped<IInvestmentCalculationService, InvestmentCalculationService>();
            services.AddScoped<IInvestmentRepository, InvestmentRepository>(); 
        }
    }
}
