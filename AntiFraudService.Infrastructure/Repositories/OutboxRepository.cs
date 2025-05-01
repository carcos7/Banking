using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntiFraudService.Application.Interfaces;
using AntiFraudService.Domain.Entities;
using AntiFraudService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AntiFraudService.Infrastructure.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly AntiFraudDbContext _context;

        public OutboxRepository(AntiFraudDbContext context)
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
