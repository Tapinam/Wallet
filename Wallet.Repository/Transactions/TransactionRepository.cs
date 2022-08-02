using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Models;
using Wallet.Domain.Storage;
using Wallet.Repository.Generic;
using Wallet.Repository.Interaces.Transactions;

namespace Wallet.Repository.Transactions
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(IGameContext gameContext)
            : base(gameContext)
        { }

        public async Task<List<Transaction>> GetPlayerTransactionsAsync(Guid playerId)
        {
            return await this.FindAllAsync(t => t.PlayerId == playerId);
        }
    }
}
