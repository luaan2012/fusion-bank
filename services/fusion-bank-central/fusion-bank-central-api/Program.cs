using fusion.bank.central.domain.Interfaces;
using fusion.bank.central.repository;
using fusion.bank.central.service;
using fusion.bank.core.Middlewares;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBankRepository, BankRepository>();

builder.Services.AddMassTransit(busCfg =>
{
    busCfg.SetKebabCaseEndpointNameFormatter();

    busCfg.AddConsumer<NewAccountCentralConsumer>();
    busCfg.AddConsumer<NewDepositCentralConsumer>();
    busCfg.AddConsumer<NewTransferCentralConsumer>();
    busCfg.AddConsumer<NewKeyAccountCentralConsumer>();

    busCfg.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration.GetConnectionString("RabbitMQ")));

        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

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
