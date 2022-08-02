using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;
using Wallet.Domain.Models;
using Wallet.Domain.Requests;
using Wallet.Domain.Responses;
using Wallet.Services.Interfaces.Players;
using Wallet.Services.Interfaces.Transactions;

namespace Wallet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayersController : BaseController
    {
        private readonly IPlayerService _playerService;
        private readonly ITransactionService _transactionService;

        public PlayersController(IPlayerService playerService, ITransactionService transactionService)
        {
            _playerService = playerService;
            _transactionService = transactionService;
        }

        [HttpGet("/api/players/ballance/{Id:Guid}")]
        public async Task<IActionResult> Ballance([FromRoute] PlayerRequest request)
        {
            if (request is not null && request.Id != Guid.Empty)
            {
                var ballance = await _playerService.GetBalanceAsync(request.Id);

                return new JsonResult(GetSuccessResponse<decimal>(ballance));
            }

            return new JsonResult(GetErrorResponse());
        }

        [HttpPost("/api/players/register")]
        public async Task<IActionResult> Register([FromBody] PlayerRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request?.Name))
            {
                var player = await _playerService.RegisterAsync(request.Name);

                return new JsonResult(GetPlayerResponse(player));
            }

            return new JsonResult(GetErrorResponse());
        }

        [HttpGet("/api/players/{Id:Guid}/transactions")]
        public async Task<IActionResult> Transactions([FromRoute] PlayerRequest request)
        {
            if (request is not null && request.Id != Guid.Empty)
            {
                var transactions = await _transactionService.ListPlayerTransactionsAsync(request.Id);

                return new JsonResult(GetTransactionsResponse(transactions));
            }

            return new JsonResult(GetErrorResponse());
        }
    }
}
