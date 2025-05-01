using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AntiFraudService.Application.Interfaces;
using AntiFraudService.Domain.Entities;

namespace AntiFraudService.Application.Services
{
    public class ValidateTransactionService
    {
        private readonly ITransactionValidationRepository _repository;
        private readonly IOutboxRepository _outboxRepository;

        public ValidateTransactionService(
            ITransactionValidationRepository repository,
            IOutboxRepository outboxRepository)
        {
            _repository = repository;
            _outboxRepository = outboxRepository;
        }

        public async Task Execute(Guid transactionId, Guid accountId, decimal amount, DateTime transactionDate)
        {
            var validation = new TransactionValidation
            {
                TransactionId = transactionId,
                AccountId = accountId,
                Amount = amount,
                TransactionDate = transactionDate
            };

            await _repository.CreateAsync(validation);

            string status;
            string? reason = null;
            decimal dailyTotal = 0;

            // Rule 1: Single transaction > 2000
            if (amount > 2000)
            {
                status = "rejected";
                reason = "Single transaction exceeds limit of 2000";
            }
            else
            {
                // Rule 2: Daily accumulated > 20000
                dailyTotal = await _repository.GetDailyTotalAsync(accountId, transactionDate.Date);
                if (dailyTotal + amount > 20000)
                {
                    status = "rejected";
                    reason = "Daily accumulated transactions exceed limit of 20000";
                }
                else
                {
                    status = "approved";
                }
            }

            validation.Status = status;
            validation.Reason = reason;
            validation.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();

            // Publish validation result
            await _outboxRepository.AddAsync(new OutboxMessage
            {
                Type = "TransactionValidated",
                Content = JsonSerializer.Serialize(new
                {
                    TransactionId = transactionId,
                    Status = status,
                    Reason = reason,
                    ValidatedAt = DateTime.UtcNow,
                    AccountId = accountId,
                    Amount = amount,
                    DailyTotal = dailyTotal + amount
                })
            });
        }
    }
}
