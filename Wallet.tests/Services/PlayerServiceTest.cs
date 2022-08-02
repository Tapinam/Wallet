using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Models;
using Wallet.Repository.Interaces.Players;
using Wallet.Services.Interfaces.Players;
using Wallet.Services.Players;
using Wallet.UnitTests.Base;

namespace Wallet.UnitTests.Services
{
    [TestFixture]
    public class PlayerServiceTest : BaseTests
    {
        private readonly Mock<IPlayerRepository> _playerRepository = new Mock<IPlayerRepository>();

        private IPlayerService _playerService;

        [SetUp]
        public void SetUp()
        {
            _playerService = new PlayerService(_playerRepository.Object);

            _playerRepository.Setup(pr => pr.GetPlayerBalanceAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid playerId) =>
                {
                    var player = GameContext.Players.Find(p => p.Id == playerId);
                    return player?.Wallet == null ? 0 : player.Wallet.Ballance;
                });

            _playerRepository.Setup(pr => pr.FindAsync(It.IsAny<Predicate<Player>>()))
                .ReturnsAsync((Predicate<Player> predicate) => GameContext.Players.Find(predicate));

            _playerRepository.Setup(pr => pr.AddAsync(It.IsAny<Player>()))
                .ReturnsAsync((Player player) =>
                {
                    GameContext.Players.Add(player);
                    return player;
                });
        }

        [TearDown]
        public void TearDown()
        {
            _playerRepository.Invocations.Clear();
        }

        [Test]
        public async Task GetBalance_ByPlayerId_WalletBallanceReturned()
        {
            var player = GameContext.Players.First();

            var ballance = await _playerService.GetBalanceAsync(player.Id);

            Assert.NotZero(ballance);
            Assert.That(ballance, Is.EqualTo(400));
        }

        [Test]
        public async Task GetBalance_ByPlayer_WalletBallanceReturned()
        {
            var player = GameContext.Players.First();

            var ballance = await _playerService.GetBalanceAsync(player);

            Assert.NotZero(ballance);
            Assert.That(ballance, Is.EqualTo(400));
        }

        [Test]
        public async Task GetBalance_ByUnknownPlayer_NoBallanceReturned()
        {
            var player = new Player { Id = Guid.NewGuid(), Name = "Jim", Wallet = new PlayerWallet() };

            var ballance = await _playerService.GetBalanceAsync(player.Id);

            Assert.Zero(ballance);
            Assert.That(ballance, Is.EqualTo(0));
        }

        [Test]
        public async Task Register_NewPlayer_PlayerRegistered()
        {
            var name = "Jim";

            var registeredPlayer = await _playerService.RegisterAsync(name);

            Assert.NotNull(registeredPlayer);
            Assert.That(registeredPlayer.Name, Is.EqualTo(name));
        }

        [Test]
        public async Task Register_NewPlayer_PlayerWalletInitialized()
        {
            var name = "Jim";

            var registeredPlayer = await _playerService.RegisterAsync(name);

            Assert.NotNull(registeredPlayer);
            Assert.That(registeredPlayer.Name, Is.EqualTo(name));
            Assert.Zero(registeredPlayer.Wallet.Ballance);
        }

        [Test]
        public async Task Register_ExistingPlayer_ExistingPlayerReturned()
        {
            var player = GameContext.Players.First();

            var registeredPlayer = await _playerService.RegisterAsync(player.Name);

            Assert.NotNull(registeredPlayer);
            Assert.That(player.Name, Is.EqualTo(registeredPlayer.Name));
            Assert.That(registeredPlayer.Name, Is.EqualTo("Alice"));
            Assert.That(registeredPlayer.Wallet.Ballance, Is.EqualTo(400));
        }
    }
}
