using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Enums;
using Wallet.Domain.Models;

namespace Wallet.Services.Interfaces.Transactions
{
    public interface ITransactionService
    {
        Task<TransactionStatus> CommitTransactionAsync(Guid playedId, Transaction transaction);
        Task<TransactionStatus> CommitTransactionAsync(Player player, Transaction transaction);
        Task<List<Transaction>> ListPlayerTransactionsAsync(Guid playerId);
        Task<List<Transaction>> ListPlayerTransactionsAsync(Player player);

    }
}
