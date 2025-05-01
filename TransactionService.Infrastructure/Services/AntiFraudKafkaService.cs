using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TransactionService.Application.Interfaces;
using TransactionService.Domain.Entities;

namespace TransactionService.Infrastructure.Services
{
    public class AntiFraudKafkaService : IAntiFraudService
    {
        private readonly IOutboxRepository _outboxRepository;

        public AntiFraudKafkaService(IOutboxRepository outboxRepository)
        {
            _outboxRepository = outboxRepository;
        }

        public async Task ValidateTransactionAsync(Transaction transaction)
        {
            await _outboxRepository.AddAsync(new OutboxMessage
            {
                Type = "TransactionValidation",
                Content = JsonSerializer.Serialize(new
                {
                    TransactionId = transaction.TransactionExternalId,
                    transaction.SourceAccountId,
                    transaction.Value,
                    transaction.CreatedAt
                })
            });
        }
    }
}
