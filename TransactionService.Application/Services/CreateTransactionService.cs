using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Application.DTOs;
using TransactionService.Application.Interfaces;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Services
{
    public class CreateTransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAntiFraudService _antiFraudService;

        public CreateTransactionService(
            ITransactionRepository transactionRepository,
            IAntiFraudService antiFraudService)
        {
            _transactionRepository = transactionRepository;
            _antiFraudService = antiFraudService;
        }

        public async Task<TransactionResponseDto> Execute(CreateTransactionDto dto)
        {
            var transaction = new Transaction
            {
                TransactionExternalId = Guid.NewGuid(),
                SourceAccountId = dto.SourceAccountId,
                TargetAccountId = dto.TargetAccountId,
                TransferTypeId = dto.TransferTypeId,
                Value = dto.Value
            };

            await _transactionRepository.CreateAsync(transaction);
            await _antiFraudService.ValidateTransactionAsync(transaction);

            return new TransactionResponseDto
            {
                TransactionExternalId = transaction.TransactionExternalId,
                CreatedAt = transaction.CreatedAt,
                Status = transaction.Status
            };
        }
    }
}
