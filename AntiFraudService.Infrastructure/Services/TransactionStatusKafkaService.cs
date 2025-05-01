using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AntiFraudService.Application.Interfaces;
using AntiFraudService.Domain.Entities;

namespace AntiFraudService.Infrastructure.Services
{
    public class TransactionStatusKafkaService : ITransactionStatusService
    {
        private readonly IOutboxRepository _outboxRepository;

        public TransactionStatusKafkaService(IOutboxRepository outboxRepository)
        {
            _outboxRepository = outboxRepository;
        }

        public async Task UpdateTransactionStatusAsync(Guid transactionId, string status)
        {
            await _outboxRepository.AddAsync(new OutboxMessage
            {
                Type = "TransactionStatusUpdated",
                Content = JsonSerializer.Serialize(new
                {
                    TransactionId = transactionId,
                    Status = status,
                    UpdatedAt = DateTime.UtcNow
                })
            });
        }
    }
}
