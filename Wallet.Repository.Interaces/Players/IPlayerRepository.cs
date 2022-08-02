using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Models;
using Wallet.Repository.Interaces.Generic;

namespace Wallet.Repository.Interaces.Players
{
    public interface IPlayerRepository : IGenericRepository<Player>
    {
        Task<decimal> GetPlayerBalanceAsync(Guid playerId);
    }
}
