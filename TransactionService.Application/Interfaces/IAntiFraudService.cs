using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Interfaces
{
    public interface IAntiFraudService
    {
        Task ValidateTransactionAsync(Transaction transaction);
    }
}
