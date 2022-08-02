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

namespace Wallet.Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPlayerRepository _playerRepository;

        public TransactionService(ITransactionRepository transactionRepository, 
            IPlayerRepository playerRepository)
        {
            _transactionRepository = transactionRepository;
            _playerRepository = playerRepository;
        }

        public async Task<TransactionStatus> CommitTransactionAsync(Guid playedId, Transaction transaction)
        {
            transaction.PlayerId = playedId;
            var existingTransaction = await _transactionRepository.FindAsync(t => t.IdempotencyKey == transaction.IdempotencyKey);

            if (existingTransaction is null)
            {
                transaction.TransactionStatus = await ChangeBalanceAsync(transaction) ?
                    TransactionStatus.Accepted : TransactionStatus.Rejected;
                existingTransaction = await _transactionRepository.AddAsync(transaction);
            }
            return existingTransaction.TransactionStatus;
        }

        public async Task<TransactionStatus> CommitTransactionAsync(Player player, Transaction transaction)
        {
            return await CommitTransactionAsync(player.Id, transaction);
        }

        public async Task<List<Transaction>> ListPlayerTransactionsAsync(Guid playerId)
        {
            return await _transactionRepository.GetPlayerTransactionsAsync(playerId);
        }

        public async Task<List<Transaction>> ListPlayerTransactionsAsync(Player player)
        {
            return await ListPlayerTransactionsAsync(player.Id);
        }

        private async Task<bool> ChangeBalanceAsync(Transaction transaction)
        {
            var player = await _playerRepository.FindAsync(p => p.Id == transaction.PlayerId);

            if(player == null)
                return false;

            var opAmount = transaction.TransactionType == TransactionType.Stake ? 
                -1 * transaction.Amount :
                transaction.Amount;

            var isValid = player.Wallet.Ballance >= 0 &&
                player.Wallet.Ballance + opAmount >= 0;

            if(isValid)
                player.Wallet.Ballance += opAmount;

            return isValid;
        }
    }
}
