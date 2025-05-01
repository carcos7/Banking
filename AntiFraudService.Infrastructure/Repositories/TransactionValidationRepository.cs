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
    public class TransactionValidationRepository : ITransactionValidationRepository
    {
        private readonly AntiFraudDbContext _context;

        public TransactionValidationRepository(AntiFraudDbContext context)
        {
            _context = context;
        }

        public async Task<TransactionValidation> CreateAsync(TransactionValidation validation)
        {
            await _context.TransactionValidations.AddAsync(validation);
            return validation;
        }

        public async Task<decimal> GetDailyTotalAsync(Guid accountId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return await _context.TransactionValidations
                .Where(v => v.AccountId == accountId &&
                           v.TransactionDate >= startDate &&
                           v.TransactionDate < endDate &&
                           v.Status == "approved")
                .SumAsync(v => v.Amount);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
