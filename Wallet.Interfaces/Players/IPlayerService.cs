using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Models;

namespace Wallet.Services.Interfaces.Players
{
    public interface IPlayerService
    {
        Task<Player> RegisterAsync(string name);
        Task<decimal> GetBalanceAsync(Guid playerId);
        Task<decimal> GetBalanceAsync(Player player);
    }
}
