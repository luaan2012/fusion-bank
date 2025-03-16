using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Middlewares;
using fusion.bank.transfer.domain.Interfaces;
using fusion.bank.transfer.repository;
using fusion.bank.transfer.services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITransferRepository, TransferRepository>();
builder.Services.AddHostedService<TransferBackground>();

builder.Services.AddMassTransit(busCfg =>
{
    busCfg.SetKebabCaseEndpointNameFormatter();

    busCfg.AddRequestClient<NewTransferAccountRequest>();

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
