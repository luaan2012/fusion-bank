using System.Text.Json.Serialization;
using fusion.bank.core.Autentication;
using fusion.bank.core.Configuration;
using fusion.bank.core.Middlewares;
using fusion.bank.creditcard.api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthenticationHandle(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddCorsApis();

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

app.UseAuthenticationApis();

app.MapControllers();

app.Run();