using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Models;
using Wallet.Domain.Storage;
using Wallet.Repository.Generic;
using Wallet.Repository.Interaces.Players;

namespace Wallet.Repository.Players
{
    public class PlayerRepository : GenericRepository<Player>, IPlayerRepository
    {
        public PlayerRepository(IGameContext gameContext)
            : base(gameContext)
        { }

        public async Task<decimal> GetPlayerBalanceAsync(Guid playerId)
        {
            var player = await FindAsync(p => p.Id == playerId);

            if (player == null)
                return 0;

            return player.Wallet.Ballance;
        }
    }
}
