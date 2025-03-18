using System.Text.Json.Serialization;
using fusion.bank.central.repository;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Middlewares;
using fusion.bank.creditcard.domain.Interfaces;
using MassTransit;
using fusion.bank.creditcard.services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); }); ;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICreditCartRepository, CreditCardRepository>();

builder.Services.AddScoped<IGenerateCreditCardService, GenerateCreditCardService>();

builder.Services.AddMassTransit(busCfg =>
{
    busCfg.SetKebabCaseEndpointNameFormatter();

    busCfg.AddRequestClient<NewCreditCardRequest>();
    busCfg.AddRequestClient<NewCreditCardCreatedRequest>();

    busCfg.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration.GetConnectionString("RabbitMQ")));

        cfg.ConfigureEndpoints(ctx);
    });
});


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();