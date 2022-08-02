using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Enums;
using Wallet.Domain.Models;
using Wallet.Repository.Interaces.Players;
using Wallet.Repository.Interaces.Transactions;
using Wallet.Services.Interfaces.Transactions;
using Wallet.Services.Transactions;
using Wallet.UnitTests.Base;

namespace Wallet.UnitTests.Services
{
    [TestFixture]
    public class TransactionServiceTest : BaseTests
    {
        private readonly Mock<ITransactionRepository> _transactionRepository = new Mock<ITransactionRepository>();
        private readonly Mock<IPlayerRepository> _playerRepository = new Mock<IPlayerRepository>();

        private ITransactionService _transactionService;
        private Player _newPlayer;

        [SetUp]
        public void SetUp()
        {
            _newPlayer = new Player { Id = Guid.NewGuid(), Name = "Jim", Wallet = new PlayerWallet { Ballance = 100 } };

            _transactionService = new TransactionService(_transactionRepository.Object, _playerRepository.Object);

            _transactionRepository.Setup(tr => tr.GetPlayerTransactionsAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid playerId) => GameContext.Transactions.FindAll(itr => itr.PlayerId == playerId));

            _transactionRepository.Setup(tr => tr.FindAsync(It.IsAny<Predicate<Transaction>>()))
                .ReturnsAsync((Predicate<Transaction> predicate) => GameContext.Transactions.Find(predicate));

            _transactionRepository.Setup(tr => tr.AddAsync(It.IsAny<Transaction>()))
                .ReturnsAsync((Transaction transaction) =>
                {
                    GameContext.Transactions.Add(transaction);
                    return transaction;
                });

            _playerRepository.Setup(pr => pr.FindAsync(It.IsAny<Predicate<Player>>()))
                .ReturnsAsync((Predicate<Player> predicate) => GameContext.Players.Find(predicate));

        }

        [TearDown]
        public void TearDown()
        {
            _playerRepository.Invocations.Clear();
            _transactionRepository.Invocations.Clear();
        }

        [Test]
        public async Task CommitTransaction_ByPlayerId_TransactionAccepted()
        {
            var player = GameContext.Players.First();
            var transaction = new Transaction
            {
                Amount = 450,
                TransactionType = TransactionType.Win
            };

            var status = await _transactionService.CommitTransactionAsync(player.Id, transaction);

            Assert.That(status, Is.EqualTo(TransactionStatus.Accepted));

            _transactionRepository.Invocations.Clear();
            _playerRepository.Invocations.Clear();
        }

        [Test]
        public async Task CommitTransaction_ByPlayer_TransactionAccepted()
        {
            var player = GameContext.Players.First();
            var transaction = new Transaction
            {
                Amount = 450,
                TransactionType = TransactionType.Win
            };

            var status = await _transactionService.CommitTransactionAsync(player, transaction);

            Assert.That(status, Is.EqualTo(TransactionStatus.Accepted));

            _transactionRepository.Invocations.Clear();
            _playerRepository.Invocations.Clear();
        }

        [Test]
        public async Task CommitTransaction_ByPlayerId_TransactionRejected()
        {
            var player = GameContext.Players.First();
            var transaction = new Transaction
            {
                Amount = 650,
                TransactionType = TransactionType.Stake
            };

            var status = await _transactionService.CommitTransactionAsync(player.Id, transaction);

            Assert.That(status, Is.EqualTo(TransactionStatus.Rejected));

            _transactionRepository.Invocations.Clear();
            _playerRepository.Invocations.Clear();
        }

        [Test]
        public async Task CommitTransaction_ByPlayer_TransactionRejected()
        {
            var player = GameContext.Players.First();
            var transaction = new Transaction
            {
                Amount = 650,
                TransactionType = TransactionType.Stake
            };

            var status = await _transactionService.CommitTransactionAsync(player, transaction);

            Assert.That(status, Is.EqualTo(TransactionStatus.Rejected));

            _transactionRepository.Invocations.Clear();
            _playerRepository.Invocations.Clear();
        }

        [Test]
        public async Task CommitTransaction_SimilarRequest_ExistingAcceptedTransactionReturned()
        {
            var player = GameContext.Players.First();
            var transaction1 = new Transaction
            {
                Amount = 50,
                TransactionType = TransactionType.Win
            };

            var transaction2 = new Transaction
            {
                Amount = 50,
                TransactionType = TransactionType.Win
            };

            var status1 = await _transactionService.CommitTransactionAsync(player.Id, transaction1);
            var status2 = await _transactionService.CommitTransactionAsync(player.Id, transaction2);

            Assert.That(transaction1.IdempotencyKey, Is.EqualTo(transaction2.IdempotencyKey));
            Assert.That(status1, Is.EqualTo(status2));
            Assert.That(status1, Is.EqualTo(TransactionStatus.Accepted));
            Assert.That(status2, Is.EqualTo(TransactionStatus.Accepted));

            _transactionRepository.Invocations.Clear();
            _playerRepository.Invocations.Clear();
        }

        [Test]
        public async Task CommitTransaction_SimilarRequest_ExistingRejectedTransactionReturned()
        {
            var transaction1 = new Transaction
            {
                Amount = 150,
                TransactionType = TransactionType.Stake
            };

            var transaction2 = new Transaction
            {
                Amount = 150,
                TransactionType = TransactionType.Stake
            };

            var status1 = await _transactionService.CommitTransactionAsync(_newPlayer.Id, transaction1);
            var status2 = await _transactionService.CommitTransactionAsync(_newPlayer.Id, transaction2);

            Assert.That(transaction1.IdempotencyKey, Is.EqualTo(transaction2.IdempotencyKey));
            Assert.That(status1, Is.EqualTo(status2));
            Assert.That(status1, Is.EqualTo(TransactionStatus.Rejected));
            Assert.That(status2, Is.EqualTo(TransactionStatus.Rejected));

            _transactionRepository.Invocations.Clear();
            _playerRepository.Invocations.Clear();
        }

        [Test]
        public async Task ListTransactions_ByPlayerId_CorrectTransactionListReturned()
        {
            var player = GameContext.Players.First();

            var transactions = await _transactionService.ListPlayerTransactionsAsync(player.Id);

            Assert.NotNull(transactions);
            Assert.NotZero(transactions.Count);
            Assert.That(transactions[0].Amount, Is.EqualTo(100));
            Assert.That(transactions[1].Amount, Is.EqualTo(100));
            Assert.That(transactions[2].Amount, Is.EqualTo(400));

            _transactionRepository.Invocations.Clear();
            _playerRepository.Invocations.Clear();
        }

        [Test]
        public async Task ListTransactions_ByPlayer_CorrectTransactionListReturned()
        {
            var player = GameContext.Players.First();

            var transactions = await _transactionService.ListPlayerTransactionsAsync(player);

            Assert.NotNull(transactions);
            Assert.NotZero(transactions.Count);
            Assert.That(transactions[0].Amount, Is.EqualTo(100));
            Assert.That(transactions[1].Amount, Is.EqualTo(100));
            Assert.That(transactions[2].Amount, Is.EqualTo(400));

            _transactionRepository.Invocations.Clear();
            _playerRepository.Invocations.Clear();
        }

        [Test]
        public async Task ListTransactions_ByPlayerId_TransactionListNotFound()
        {
            var transactions = await _transactionService.ListPlayerTransactionsAsync(_newPlayer.Id);

            Assert.Zero(transactions.Count);

            _transactionRepository.Invocations.Clear();
            _playerRepository.Invocations.Clear();
        }

        [Test]
        public async Task ListTransactions_ByPlayer_TransactionListNotFound()
        {
            var transactions = await _transactionService.ListPlayerTransactionsAsync(_newPlayer);

            Assert.Zero(transactions.Count);

            _transactionRepository.Invocations.Clear();
            _playerRepository.Invocations.Clear();
        }
    }
}
