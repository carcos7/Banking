using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AntiFraudService.Application.Services;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AntiFraudService.Infrastructure.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly string _topic;

        public KafkaConsumerService(
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            ILogger<KafkaConsumerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            var config = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = configuration["Kafka:ConsumerGroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _topic = configuration["Kafka:Topics:TransactionCreated"];
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_topic);
            _logger.LogInformation("Started consuming topic: {Topic}", _topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    _logger.LogDebug("Received message: {Message}", consumeResult.Message.Value);
                    var message = JsonSerializer.Deserialize<TransactionCreatedEvent>(consumeResult.Message.Value);

                    using var scope = _serviceProvider.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<ValidateTransactionService>();

                    await service.Execute(
                        message.TransactionId,
                        message.SourceAccountId,
                        message.Value,
                        message.CreatedAt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing transaction creation message");
                }
            }
        }

        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }

        private record TransactionCreatedEvent(
            Guid TransactionId,
            Guid SourceAccountId,
            decimal Value,
            DateTime CreatedAt);
    }
}
