using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Models;
using Wallet.Repository.Interaces.Players;
using Wallet.Services.Interfaces.Players;

namespace Wallet.Services.Players
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<decimal> GetBalanceAsync(Guid playerId)
        {
            return await _playerRepository.GetPlayerBalanceAsync(playerId);
        }

        public async Task<decimal> GetBalanceAsync(Player player)
        {
            return await GetBalanceAsync(player.Id);
        }

        public async Task<Player> RegisterAsync(string name)
        {
            var player = await _playerRepository.FindAsync(p => p.Name == name);

            if(player == null)
                return await _playerRepository.AddAsync(new Player { Id = Guid.NewGuid(), Name = name, Wallet = new PlayerWallet() });

            return player;
        }
    }
}
