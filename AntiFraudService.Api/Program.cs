using AntiFraudService.Application.Interfaces;
using AntiFraudService.Application.Services;
using AntiFraudService.Infrastructure.Context;
using AntiFraudService.Infrastructure.Repositories;
using AntiFraudService.Infrastructure.Services;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<AntiFraudDbContext>(options =>

options.UseNpgsql(builder.Configuration.GetConnectionString("AntiFraudDb"),
    npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
        
    }));

builder.Services.AddScoped<ITransactionValidationRepository, TransactionValidationRepository>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddScoped<ITransactionStatusService, TransactionStatusKafkaService>();
builder.Services.AddScoped<ValidateTransactionService>();

// Kafka producer
builder.Services.AddSingleton<IProducer<Null, string>>(sp =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = sp.GetRequiredService<IConfiguration>()["Kafka:BootstrapServers"]
    };
    return new ProducerBuilder<Null, string>(config).Build();
});

// Background services
builder.Services.AddHostedService<OutboxPublisherService>();
builder.Services.AddHostedService<KafkaConsumerService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Anti-Fraud Service API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Anti-Fraud Service v1");
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();
app.MapControllers();

app.Run();