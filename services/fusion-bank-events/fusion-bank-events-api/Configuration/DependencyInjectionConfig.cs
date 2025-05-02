using System.Text.Json.Serialization;
using fusion.bank.events.domain.Interfaces;
using fusion.bank.events.repository;

namespace fusion.bank.events.api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IEventRepository, EventRepository>();

            services.AddSignalR().AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }); ;
        }
    }
}
