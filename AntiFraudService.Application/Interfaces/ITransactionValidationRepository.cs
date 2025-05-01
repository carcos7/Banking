using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntiFraudService.Domain.Entities;

namespace AntiFraudService.Application.Interfaces
{
    public interface ITransactionValidationRepository
    {
        Task<TransactionValidation> CreateAsync(TransactionValidation validation);
        Task<decimal> GetDailyTotalAsync(Guid accountId, DateTime date);
        Task SaveChangesAsync();
    }
}
