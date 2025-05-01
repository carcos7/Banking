using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Interfaces;

namespace TransactionService.Infrastructure.Services
{
    public class OutboxPublisherService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxPublisherService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

        public OutboxPublisherService(
            IServiceProvider serviceProvider,
            ILogger<OutboxPublisherService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                    var producer = scope.ServiceProvider.GetRequiredService<IProducer<Null, string>>();
                    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                    var pendingMessages = await outboxRepository.GetPendingMessagesAsync();

                    foreach (var message in pendingMessages)
                    {
                        try
                        {
                            var topic = message.Type switch
                            {
                                "TransactionCreated" => config["Kafka:Topics:TransactionCreated"],
                                "TransactionValidation" => config["Kafka:Topics:TransactionValidated"],
                                _ => throw new InvalidOperationException($"Unknown message type: {message.Type}")
                            };

                            await producer.ProduceAsync(topic, new Message<Null, string> { Value = message.Content });
                            await outboxRepository.MarkAsProcessedAsync(message.Id);
                            _logger.LogInformation("Published message {MessageId} to {Topic}", message.Id, topic);
                        }
                        catch (Exception ex)
                        {
                            await outboxRepository.MarkAsFailedAsync(message.Id, ex.Message);
                            _logger.LogError(ex, "Failed to publish message {MessageId}", message.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing outbox messages");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
