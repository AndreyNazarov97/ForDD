using ForDD.DAL.DependencyInjection;
using ForDD.Application.DependencyInjection;
using Serilog;
using ForDD.API;
using ForDD.Domain.Settings;
using ForDD.API.Middlewares;
using ForDD.Producer.DependencyInjection;
using ForDD.Consumer.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSection));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));

builder.Services.AddControllers();

builder.Services.AddAuthenticationAndAuthorization(builder);
builder.Services.AddSwagger();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ForDD_";
} );

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));


builder.Services.AddDataAccesLayer(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddProducer();
builder.Services.AddConsumer();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI( c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ForDD Swagger v 1.0");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "ForDD Swagger v 2.0");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.MapControllers();
    
app.Run();


