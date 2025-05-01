using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Interfaces;
using TransactionService.Domain.Entities;
using TransactionService.Infrastructure.Context;

namespace TransactionService.Infrastructure.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly TransactionDbContext _context;

        public OutboxRepository(TransactionDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OutboxMessage message)
        {
            await _context.OutboxMessages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize = 100)
        {
            return await _context.OutboxMessages
                .Where(m => m.ProcessedAt == null)
                .OrderBy(m => m.CreatedAt)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task MarkAsProcessedAsync(Guid messageId)
        {
            var message = await _context.OutboxMessages.FindAsync(messageId);
            if (message != null)
            {
                message.ProcessedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAsFailedAsync(Guid messageId, string error)
        {
            var message = await _context.OutboxMessages.FindAsync(messageId);
            if (message != null)
            {
                message.Error = error;
                await _context.SaveChangesAsync();
            }
        }
    }
}
