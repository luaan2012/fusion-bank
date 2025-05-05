using System.Text.Json.Serialization;
using fusion.bank.account.api.Configuration;
using fusion.bank.core.Autentication;
using fusion.bank.core.Configuration;
using fusion.bank.core.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthenticationHandle(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddSwaggerConfig(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.AllowAnyOrigin() // Origem do front-end
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

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

app.Run();
