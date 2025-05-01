using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiFraudService.Application.Interfaces
{
    public interface ITransactionStatusService
    {
        Task UpdateTransactionStatusAsync(Guid transactionId, string status);
    }
}
