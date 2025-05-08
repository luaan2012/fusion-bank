using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace fusion.bank.core.Configuration
{
    public static class AutenticationConfiguration
    {
        public static void AddCorsApis(this IServiceCollection service)
        {
            service.AddCors(options =>
             {
                 options.AddPolicy("AllowSpecificOrigin", policy =>
                 {
                     policy.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                 });
             });
        }

        public static void UseCorsApis(this WebApplication app)
        {
            app.UseCors("AllowSpecificOrigin");
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
