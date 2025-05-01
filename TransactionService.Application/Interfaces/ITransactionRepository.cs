using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction?> GetByIdAsync(Guid transactionId);
        Task UpdateStatusAsync(Guid transactionId, string status);
        Task<decimal> GetDailyTotalAsync(Guid accountId, DateTime date);
    }
}
