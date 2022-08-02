using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Models;
using Wallet.Repository.Interaces.Players;
using Wallet.Repository.Players;
using Wallet.UnitTests.Base;

namespace Wallet.UnitTests.Repositories
{
    [TestFixture]
    public class PlayerRepositoryTest : BaseTests
    {
        private IPlayerRepository _playerRepository;

        private Player _newPlayer;

        [SetUp]
        public void SetUp()
        {
            _newPlayer = new Player { Id = Guid.NewGuid(), Name = "Jim", Wallet = new PlayerWallet() };
            
            _playerRepository = new PlayerRepository(GameContext);
        }

        [Test]
        public async Task GetPlayerBalance_PlayerFound_WalletHasCorrectValue()
        {
            var player = GameContext.Players.First(p => p.Name == "Alice");

            var foundPlayer = await _playerRepository.FindAsync(p => p.Name == "Alice");

            Assert.IsNotNull(foundPlayer);
            Assert.That(foundPlayer.Id, Is.EqualTo(player.Id));
            Assert.That(foundPlayer.Wallet.Ballance, Is.EqualTo(player.Wallet.Ballance));
        }

        [Test]
        public async Task AddNewPlayer_NewPlayerName_PlayerAdded()
        {
            var addedPlayer = await _playerRepository.AddAsync(_newPlayer);

            Assert.That(addedPlayer.Id, Is.EqualTo(_newPlayer.Id));
            Assert.That(addedPlayer.Name, Is.EqualTo(_newPlayer.Name));
            Assert.Zero(addedPlayer.Wallet.Ballance);
        }
    }
}
