using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Models;

namespace Wallet.Domain.Storage
{
    public class GameContext : IGameContext
    {
        public GameContext()
        {
            Players = new GameCollection<Player>();
            Transactions = new GameCollection<Transaction>();
        }

        public GameCollection<Player> Players
        {
            get { return Game.Instance.GetCollection<Player>(); }
            private set { Game.Instance.SetCollection(value); }
        }

        public GameCollection<Transaction> Transactions
        {
            get { return Game.Instance.GetCollection<Transaction>(); }
            private set { Game.Instance.SetCollection(value); }
        }

        public GameCollection<TModel> Collection<TModel>()
        {
            return Game.Instance.GetCollection<TModel>();
        }

        public void Reset()
        {
            Players.Reset();
            Transactions.Reset();
            Game.Instance.Dispose();
        }
    }
}
