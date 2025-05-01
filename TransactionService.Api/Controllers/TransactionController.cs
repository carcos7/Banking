using Microsoft.AspNetCore.Mvc;
using TransactionService.Application.DTOs;
using TransactionService.Application.Interfaces;
using TransactionService.Application.Services;

namespace TransactionService.Api.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly CreateTransactionService _createTransactionService;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionController(
            CreateTransactionService createTransactionService,
            ITransactionRepository transactionRepository)
        {
            _createTransactionService = createTransactionService;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto dto)
        {
            var result = await _createTransactionService.Execute(dto);
            return Ok(result);
        }

        [HttpGet("{transactionExternalId}")]
        public async Task<IActionResult> GetTransaction(Guid transactionExternalId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionExternalId);

            if (transaction == null)
                return NotFound();

            return Ok(new TransactionResponseDto
            {
                TransactionExternalId = transaction.TransactionExternalId,
                CreatedAt = transaction.CreatedAt,
                Status = transaction.Status
            });
        }
    }
}
