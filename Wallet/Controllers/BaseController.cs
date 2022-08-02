using Microsoft.AspNetCore.Mvc;
using Wallet.Domain.Models;
using Wallet.Domain.Responses;

namespace Wallet.Controllers
{
    public class BaseController : ControllerBase
    {
        protected ResponseWrapper<string> GetErrorResponse()
        {
            return new ResponseWrapper<string>
            {
                Payload = string.Empty,
                Message = "Bad request",
                StatusCode = 400
            };
        }

        protected ResponseWrapper<List<Transaction>> GetTransactionsResponse(List<Transaction> transactions)
            => GetSuccessResponse<List<Transaction>>(transactions);

        protected ResponseWrapper<Player> GetPlayerResponse(Player player)
            => GetSuccessResponse<Player>(player);

        protected ResponseWrapper<T> GetSuccessResponse<T>(T responsePayload)
        {
            return new ResponseWrapper<T>
            {
                Payload = responsePayload,
                Message = "Success",
                StatusCode = 200
            };
        }
    }
}
