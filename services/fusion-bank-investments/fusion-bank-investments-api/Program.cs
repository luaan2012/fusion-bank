using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Middlewares;
using fusion.bank.investments.domain.Interfaces;
using fusion.bank.investments.repository;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IInvestmentRepository, InvestmentRepository>();

builder.Services.AddMassTransit(busCfg =>
{
    busCfg.SetKebabCaseEndpointNameFormatter();

    busCfg.AddRequestClient<NewAccountRequestInformation>();
    busCfg.AddRequestClient<NewInvestmentRequest>();
    busCfg.AddRequestClient<NewAccountRequestPutAmount>();

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