using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Enums;
using Wallet.Domain.Models;
using Wallet.Domain.Storage;
using Wallet.Repository.Interaces.Transactions;
using Wallet.Repository.Transactions;
using Wallet.UnitTests.Base;

namespace Wallet.UnitTests.Repositories
{
    [TestFixture]
    public class TransactionRepositoryTest : BaseTests
    {
        private ITransactionRepository _transactionRepository;

        private Player _newPlayer;

        [SetUp]
        public void SetUp()
        {
            _newPlayer = new Player { Id = Guid.NewGuid(), Name = "Jim", Wallet = new PlayerWallet { Ballance = 100 } };

            _transactionRepository = new TransactionRepository(GameContext);
        }

        [Test]
        public async Task GetPlayerTransactions_UnknownPlayer_TransactionsEmpty()
        {
            var transactions = await _transactionRepository.GetPlayerTransactionsAsync(_newPlayer.Id);

            Assert.IsFalse(transactions.Any());
        }

        [Test]
        public async Task GetPlayerTransactions_KnownPlayer_TransactionsFilled()
        {
            var player = GameContext.Players.First();

            var transactions = await _transactionRepository.GetPlayerTransactionsAsync(player.Id);

            Assert.IsTrue(transactions.Any());
            Assert.IsTrue(transactions.First().PlayerId == player.Id);
        }

        [Test]
        public async Task GetPlayerTransactions_KnownPlayer_CorrectTransactionsCountReturned()
        {
            var player = GameContext.Players.First();
            var transactions = GameContext.Transactions.Where(t => t.PlayerId == player.Id).ToList();

            var transactionList = await _transactionRepository.GetPlayerTransactionsAsync(player.Id);

            Assert.IsTrue(transactionList.Any());
            Assert.IsTrue(transactionList.First().PlayerId == player.Id);
            Assert.That(transactionList.Count, Is.EqualTo(transactions.ToList().Count));
        }

        [Test]
        public async Task FindTransaction_UnknownTransaction_TransactionNotFound()
        {
            var tranaction = new Transaction();
            var targetTransaction = await _transactionRepository.FindAsync(t => t.Id == tranaction.Id);

            Assert.IsNull(targetTransaction);
        }
    }
}
