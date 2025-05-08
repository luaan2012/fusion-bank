using System.Text.Json.Serialization;
using fusion.bank.central.api.Configuration;
using fusion.bank.core.Autentication;
using fusion.bank.core.Configuration;
using fusion.bank.core.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthenticationHandle(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddCorsApis();

builder.Services.AddSwaggerConfig(builder.Configuration);

builder.Services.AddDependencyInjection();

builder.Services.AddMassTransitConfig(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCorsApis();

app.MapControllers();

app.Run();
