using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Interfaces
{
    public interface IOutboxRepository
    {
        Task AddAsync(OutboxMessage message);
        Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize = 100);
        Task MarkAsProcessedAsync(Guid messageId);
        Task MarkAsFailedAsync(Guid messageId, string error);
    }
}
