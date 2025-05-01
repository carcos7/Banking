using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntiFraudService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AntiFraudService.Infrastructure.Context
{
    public class AntiFraudDbContext : DbContext
    {
        public AntiFraudDbContext(DbContextOptions<AntiFraudDbContext> options) : base(options) { }

        public DbSet<TransactionValidation> TransactionValidations => Set<TransactionValidation>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(AntiFraudDbContext).Assembly);
        }
    }
}
