using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Controllers;
using Wallet.Domain.Models;
using Wallet.Domain.Requests;
using Wallet.Domain.Responses;
using Wallet.Services.Interfaces.Players;
using Wallet.Services.Interfaces.Transactions;
using Wallet.UnitTests.Base;

namespace Wallet.UnitTests.Controllers
{
    [TestFixture]
    public class PlayersControllerTest : BaseTests
    {
        private readonly Mock<IPlayerService> _playerService = new Mock<IPlayerService>();
        private readonly Mock<ITransactionService> _transactionService = new Mock<ITransactionService>();

        private PlayersController _playerController;
        private PlayerRequest _request;

        [SetUp]
        public void SetUp()
        {
            _request = new PlayerRequest
            {
                Id = GameContext.Players.First(p => p.Name == "Alice").Id,
                Name = GameContext.Players.First(p => p.Name == "Alice").Name
            };

            _playerController = new PlayersController(_playerService.Object, _transactionService.Object);

            _playerService.Setup(ps => ps.RegisterAsync(It.IsAny<string>()))
                .ReturnsAsync((string name) =>
                {
                    var player = GameContext.Players.FirstOrDefault(p => p.Name == name);

                    if (player == null)
                        return new Player { Id = Guid.NewGuid(), Name = name, Wallet = new PlayerWallet() };

                    return player;
                });

            _playerService.Setup(ps => ps.GetBalanceAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid playerId) =>
                {
                    var player = GameContext.Players.FirstOrDefault(p => p.Id == playerId);

                    if (player == null)
                        return 0;

                    return player.Wallet.Ballance;
                });

            _transactionService.Setup(ts => ts.ListPlayerTransactionsAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid playerId) =>
                    GameContext.Transactions.FindAll(t => t.PlayerId == playerId));
        }

        [TearDown]
        public void TearDown()
        {
            _playerService.Invocations.Clear();
            _transactionService.Invocations.Clear();
        }

        [Test]
        public async Task RegisterPlayer_RequestIsEmpty_ErrorReturned()
        {
            _request = null;

            var result = (await _playerController.Register(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<string>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_ERROR_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_ERROR_CODE));
            Assert.That(wrapper.Payload, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task RegisterPlayer_RequestPlayerNameIsEmpty_ErrorReturned()
        {
            _request.Name = string.Empty;

            var result = (await _playerController.Register(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<string>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_ERROR_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_ERROR_CODE));
            Assert.That(wrapper.Payload, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task RegisterPlayer_ValidRequest_NewPlayerReturned()
        {
            _request.Name = "Jim";

            var result = (await _playerController.Register(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<Player>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_SUCCESS_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_SUCCESS_CODE));
            Assert.That(wrapper.Payload, Is.Not.Null);
            Assert.That(wrapper.Payload.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(string.IsNullOrWhiteSpace(wrapper.Payload.Name), Is.False);
            Assert.That(wrapper.Payload.Name, Is.EqualTo(_request.Name));
        }

        [Test]
        public async Task RegisterPlayer_ValidRequest_ExistingPlayerReturned()
        {
            _request.Name = "Alice";

            var result = (await _playerController.Register(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<Player>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_SUCCESS_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_SUCCESS_CODE));
            Assert.That(wrapper.Payload, Is.Not.Null);
            Assert.That(wrapper.Payload.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(string.IsNullOrWhiteSpace(wrapper.Payload.Name), Is.False);
            Assert.That(wrapper.Payload.Name, Is.EqualTo(_request.Name));
        }

        [Test]
        public async Task GetPlayerBallance_RequestIsEmpty_ErrorReturned()
        {
            _request = null;

            var result = (await _playerController.Ballance(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<string>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_ERROR_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_ERROR_CODE));
            Assert.That(wrapper.Payload, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task GetPlayerBallance_RequestPlayerIdIsEmpty_ErrorReturned()
        {
            _request.Id = Guid.Empty;

            var result = (await _playerController.Ballance(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<string>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_ERROR_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_ERROR_CODE));
            Assert.That(wrapper.Payload, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task GetPlayerBallance_ValidRequest_CorrectValueReturned()
        {
            var result = (await _playerController.Ballance(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<decimal>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_SUCCESS_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_SUCCESS_CODE));
            Assert.That(wrapper.Payload, Is.Not.Zero);
            Assert.That(wrapper.Payload, Is.EqualTo(400));
        }

        [Test]
        public async Task GetPlayerBallance_UnknownPlayer_ZeroBallanceReturned()
        {
            _request.Name = "Jim";
            _request.Id = Guid.NewGuid();

            var result = (await _playerController.Ballance(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<decimal>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_SUCCESS_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_SUCCESS_CODE));
            Assert.That(wrapper.Payload, Is.Zero);
        }

        [Test]
        public async Task GetPlayerTransactions_RequestIsEmpty_ErrorReturned()
        {
            _request = null;

            var result = (await _playerController.Transactions(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<string>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_ERROR_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_ERROR_CODE));
            Assert.That(wrapper.Payload, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task GetPlayerTransactions_RequestPlayerIdIsEmpty_ErrorReturned()
        {
            _request.Id = Guid.Empty;

            var result = (await _playerController.Transactions(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<string>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_ERROR_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_ERROR_CODE));
            Assert.That(wrapper.Payload, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task GetPlayerTransactions_ValidRequest_CorrectValueReturned()
        {
            var result = (await _playerController.Transactions(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<List<Transaction>>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_SUCCESS_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_SUCCESS_CODE));
            Assert.That(wrapper.Payload, Is.Not.Null);
            Assert.That(wrapper.Payload.Count, Is.Not.Zero);
            Assert.That(wrapper.Payload.Count, Is.EqualTo(3));
            Assert.That(wrapper.Payload[0].PlayerId, Is.EqualTo(_request.Id));
            Assert.That(wrapper.Payload[1].PlayerId, Is.EqualTo(_request.Id));
            Assert.That(wrapper.Payload[2].PlayerId, Is.EqualTo(_request.Id));
        }

        [Test]
        public async Task GetPlayerTransactions_UnknownPlayer_NoTransactionsReturned()
        {
            _request.Name = "Jim";
            _request.Id = Guid.NewGuid();

            var result = (await _playerController.Transactions(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<List<Transaction>>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_SUCCESS_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_SUCCESS_CODE));
            Assert.That(wrapper.Payload.Count, Is.Zero);
        }
    }
}
