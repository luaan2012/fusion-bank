using System.Text.Json.Serialization;
using fusion.bank.core.Autentication;
using fusion.bank.core.Configuration;
using fusion.bank.core.Middlewares;
using fusion.bank.events.api.Configuration;
using fusion.bank.events.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthenticationHandle(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500") // Origem do front-end
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Permitir credenciais para SignalR
    });
});

builder.Services.AddSwaggerConfig(builder.Configuration);

builder.Services.AddMassTransitConfig(builder.Configuration);

builder.Services.AddDependencyInjection();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<EventHub>("/notification");

app.Run();