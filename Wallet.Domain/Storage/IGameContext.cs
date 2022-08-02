using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Models;

namespace Wallet.Domain.Storage
{
    public interface IGameContext
    {
        GameCollection<Player> Players { get; }
        GameCollection<Transaction> Transactions { get; }
        GameCollection<TModel> Collection<TModel>();
        void Reset();
    }
}
