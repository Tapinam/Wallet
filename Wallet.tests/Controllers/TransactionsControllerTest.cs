using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Controllers;
using Wallet.Domain.Enums;
using Wallet.Domain.Models;
using Wallet.Domain.Requests;
using Wallet.Domain.Responses;
using Wallet.Repository.Interaces.Players;
using Wallet.Repository.Interaces.Transactions;
using Wallet.Services.Interfaces.Transactions;
using Wallet.UnitTests.Base;

namespace Wallet.UnitTests.Controllers
{
    [TestFixture]
    public class TransactionsControllerTest : BaseTests
    {
        private readonly Mock<ITransactionService> _transactionService = new Mock<ITransactionService>();

        private TransactionsController _transactionsController;
        private TransactionRequest _request;

        [SetUp]
        public void SetUp()
        {
            _request = new TransactionRequest
            {
                Amount = 750,
                PlayerId = GameContext.Players.First().Id,
                Type = Domain.Enums.TransactionType.Stake
            };

            _transactionsController = new TransactionsController(_transactionService.Object);

            _transactionService.Setup(ts => ts.CommitTransactionAsync(It.IsAny<Guid>(), It.IsAny<Transaction>()))
                .ReturnsAsync((Guid playerId, Transaction transaction) =>
                {
                    transaction.PlayerId = playerId;

                    if(transaction.Amount == 1000)
                    {
                        return TransactionStatus.Rejected;
                    } 
                    else if (GameContext.Players.FirstOrDefault(p => p.Id == playerId) != null)
                    {
                        var tr = GameContext.Transactions.FirstOrDefault(t => t.IdempotencyKey == transaction.IdempotencyKey);
                        
                        return tr != null ? tr.TransactionStatus : TransactionStatus.Accepted;
                    }

                    return TransactionStatus.Accepted;
                });
        }

        [TearDown]
        public void TearDown()
        {
            _transactionService.Invocations.Clear();
        }

        [Test]
        public async Task RegisterTransaction_RequestIsEmpty_ErrorReturned()
        {
            _request = null;

            var result = (await _transactionsController.Register(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<string>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_ERROR_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_ERROR_CODE));
            Assert.That(wrapper.Payload, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task RegisterTransaction_RequestPlayerIdIsEmpty_ErrorReturned()
        {
            _request.PlayerId = Guid.Empty;

            var result = (await _transactionsController.Register(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<string>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_ERROR_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_ERROR_CODE));
            Assert.That(wrapper.Payload, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task RegisterTransaction_RequestAmountIsEmpty_ErrorReturned()
        {
            _request.Amount = 0;

            var result = (await _transactionsController.Register(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<string>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_ERROR_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_ERROR_CODE));
            Assert.That(wrapper.Payload, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task RegisterTransaction_RequestAmountNegative_ErrorReturned()
        {
            _request.Amount = -1;

            var result = (await _transactionsController.Register(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<string>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_ERROR_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_ERROR_CODE));
            Assert.That(wrapper.Payload, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task RegisterTransaction_ValidRequest_AcceptedTransactionReturned()
        {
            var result = (await _transactionsController.Register(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<string>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_SUCCESS_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_SUCCESS_CODE));
            Assert.That(wrapper.Payload, Is.EqualTo(TransactionStatus.Accepted.ToString()));
        }

        [Test]
        public async Task RegisterTransaction_ValidRequest_RejectedTransactionReturned()
        {
            _request.Amount = 1000;

            var result = (await _transactionsController.Register(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<string>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_SUCCESS_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_SUCCESS_CODE));
            Assert.That(wrapper.Payload, Is.EqualTo(TransactionStatus.Rejected.ToString()));
        }

        [Test]
        public async Task RegisterTransaction_ValidRequest_ExistingAcceptedTransactionReturned()
        {
            _request.Amount = 150;
            _request.Type = TransactionType.Deposit;

            var result = (await _transactionsController.Register(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<string>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_SUCCESS_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_SUCCESS_CODE));
            Assert.That(wrapper.Payload, Is.EqualTo(TransactionStatus.Accepted.ToString()));
        }

        [Test]
        public async Task RegisterTransaction_ValidRequest_ExistingRejectedTransactionReturned()
        {
            var player = GameContext.Players.Find(p => p.Name == "Charly");

            _request.PlayerId = player.Id;
            _request.Amount = 100;
            _request.Type = TransactionType.Stake;

            var result = (await _transactionsController.Register(_request) as JsonResult);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var wrapper = result.Value as ResponseWrapper<string>;

            Assert.That(string.IsNullOrWhiteSpace(wrapper.Message), Is.False);
            Assert.That(wrapper.Message, Is.EqualTo(CONTROLLER_SUCCESS_MESSAGE));
            Assert.That(wrapper.StatusCode, Is.EqualTo(CONTROLLER_SUCCESS_CODE));
            Assert.That(wrapper.Payload, Is.EqualTo(TransactionStatus.Rejected.ToString()));
        }
    }
}
