using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntiFraudService.Domain.Entities;

namespace AntiFraudService.Application.Interfaces
{
    public interface IOutboxRepository
    {
        Task AddAsync(OutboxMessage message);
        Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize = 100);
        Task MarkAsProcessedAsync(Guid messageId);
        Task MarkAsFailedAsync(Guid messageId, string error);
    }
}
