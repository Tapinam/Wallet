using Wallet.Domain.Models;
using Wallet.Domain.Storage;

namespace Wallet.UnitTests.Base
{
    [TestFixture]
    public abstract class BaseTests
    {
        protected const string CONTROLLER_ERROR_MESSAGE = "Bad request";
        protected const string CONTROLLER_SUCCESS_MESSAGE = "Success";

        protected const int CONTROLLER_ERROR_CODE = 400;
        protected const int CONTROLLER_SUCCESS_CODE = 200;

        protected IGameContext GameContext { get; private set; }
        protected List<Player> InitialPlayers { get; private set; }
        protected List<Transaction> InitialTransactions { get; private set; }

        [SetUp]
        public void InitTests()
        {
            GameContext = InitStorage();
        }

        [TearDown]
        public void Cleanup()
        {
            GameContext?.Reset();
        }


        private IGameContext InitStorage()
        {
            var gameContext = new GameContext();

            var alice = new Player { Id = Guid.NewGuid(), Name = "Alice", Wallet = new PlayerWallet { Ballance = 400 } };
            var bill = new Player { Id = Guid.NewGuid(), Name = "Bill", Wallet = new PlayerWallet() };
            var charly = new Player {Id = Guid.NewGuid(), Name = "Charly", Wallet = new PlayerWallet { Ballance = 50 } };
            var doni = new Player {Id = Guid.NewGuid(), Name = "Doni", Wallet = new PlayerWallet { Ballance = 650 } };

            InitialPlayers = new List<Player> { alice, bill, charly, doni };

            InitialTransactions = new List<Transaction>
            {
                new Transaction
                {
                    Amount = 100,
                    PlayerId = alice.Id,
                    TransactionType = Domain.Enums.TransactionType.Deposit
                },
                new Transaction
                {
                    Amount = 100,
                    PlayerId = alice.Id,
                    TransactionType = Domain.Enums.TransactionType.Stake,
                    TransactionStatus = Domain.Enums.TransactionStatus.Accepted
                },
                new Transaction
                {
                    Amount = 400,
                    PlayerId = alice.Id,
                    TransactionType = Domain.Enums.TransactionType.Win
                },
                new Transaction
                {
                    Amount = 150,
                    PlayerId = bill.Id,
                    TransactionType = Domain.Enums.TransactionType.Deposit
                },
                new Transaction
                {
                    Amount = 100,
                    PlayerId = bill.Id,
                    TransactionType = Domain.Enums.TransactionType.Stake
                },
                new Transaction
                {
                    Amount = 50,
                    PlayerId = bill.Id,
                    TransactionType = Domain.Enums.TransactionType.Stake
                },
                new Transaction
                {
                    Amount = 30,
                    PlayerId = charly.Id,
                    TransactionType = Domain.Enums.TransactionType.Deposit
                },
                new Transaction
                {
                    Amount = 20,
                    PlayerId = charly.Id,
                    TransactionType = Domain.Enums.TransactionType.Deposit
                },
                new Transaction
                {
                    Amount = 100,
                    PlayerId = charly.Id,
                    TransactionType = Domain.Enums.TransactionType.Stake,
                    TransactionStatus = Domain.Enums.TransactionStatus.Rejected
                },
                new Transaction
                {
                    Amount = 130,
                    PlayerId = doni.Id,
                    TransactionType = Domain.Enums.TransactionType.Deposit
                },
                new Transaction
                {
                    Amount = 120,
                    PlayerId = doni.Id,
                    TransactionType = Domain.Enums.TransactionType.Deposit
                },
                new Transaction
                {
                    Amount = 100,
                    PlayerId = doni.Id,
                    TransactionType = Domain.Enums.TransactionType.Stake
                },
                new Transaction
                {
                    Amount = 500,
                    PlayerId = doni.Id,
                    TransactionType = Domain.Enums.TransactionType.Win
                }
            };

            gameContext.Players.AddRange(InitialPlayers);
            gameContext.Transactions.AddRange(InitialTransactions);

            return gameContext;
        }
    }
}