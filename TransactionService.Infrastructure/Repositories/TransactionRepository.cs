using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Interfaces;
using TransactionService.Domain.Entities;
using TransactionService.Infrastructure.Context;

namespace TransactionService.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TransactionDbContext _context;
        private readonly IOutboxRepository _outboxRepository;

        public TransactionRepository(
            TransactionDbContext context,
            IOutboxRepository outboxRepository)
        {
            _context = context;
            _outboxRepository = outboxRepository;
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                await _outboxRepository.AddAsync(new OutboxMessage
                {
                    Type = "TransactionCreated",
                    Content = JsonSerializer.Serialize(new
                    {
                        TransactionId = transaction.TransactionExternalId,
                        transaction.SourceAccountId,
                        transaction.Value,
                        transaction.CreatedAt
                    })
                });

                await dbTransaction.CommitAsync();
                return transaction;
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Transaction?> GetByIdAsync(Guid transactionId)
        {
            return await _context.Transactions.FindAsync(transactionId);
        }

        public async Task UpdateStatusAsync(Guid transactionId, string status)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction != null)
            {
                transaction.Status = status;
                transaction.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetDailyTotalAsync(Guid accountId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return await _context.Transactions
                .Where(t => t.SourceAccountId == accountId &&
                           t.CreatedAt >= startDate &&
                           t.CreatedAt < endDate)
                .SumAsync(t => t.Value);
        }
    }
}
