using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wallet.Domain.Enums;
using Wallet.Domain.Models;
using Wallet.Domain.Requests;
using Wallet.Domain.Responses;
using Wallet.Services.Interfaces.Transactions;

namespace Wallet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransactionsController : BaseController
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("/api/transactions/commit")]
        public async Task<IActionResult> Register([FromBody] TransactionRequest request)
        {
            if (request is not null &&
                request.PlayerId != Guid.Empty &&
                request.Amount > 0 &&
                !string.IsNullOrWhiteSpace(Enum.GetName<TransactionType>(request.Type)))
            {
                var transaction = new Transaction
                {
                    Amount = request.Amount,
                    TransactionType = (TransactionType)request.Type,
                };
                var player = new Player { Id = request.PlayerId };

                var transactionStatus = await _transactionService.CommitTransactionAsync(player.Id, transaction);

                return new JsonResult(GetSuccessResponse<string>(transactionStatus.ToString()));
            }

            return new JsonResult(GetErrorResponse());
        }
    }
}
