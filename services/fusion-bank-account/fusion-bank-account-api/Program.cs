using fusion.bank.account.domain.Interfaces;
using fusion.bank.account.repository;
using fusion.bank.account.service;
using fusion.bank.account.Service;
using fusion.bank.core.Messages.Consumers;
using fusion.bank.core.Messages.Producers;
using MassTransit;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); }); ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddMassTransit(busCfg =>
{
    busCfg.SetKebabCaseEndpointNameFormatter();

    busCfg.AddConsumer<CreatedAccount>();
    busCfg.AddConsumer<NewDepositAccount>();

    busCfg.AddRequestClient<NewDepositCentralRequest>();

    busCfg.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration.GetConnectionString("RabbitMQ")));

        cfg.ConfigureEndpoints(ctx);
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
