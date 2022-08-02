using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Enums;

namespace Wallet.Domain.Models
{
    public class Transaction
    {
        public Transaction()
        {
            Id = Guid.NewGuid();
            DateTimeCreated = DateTimeUpdated = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public Guid PlayerId { get; set; }
        public string IdempotencyKey => GetHashCode().ToString();
        public TransactionStatus TransactionStatus { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime DateTimeUpdated { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Transaction transaction &&
                   Amount == transaction.Amount &&
                   TransactionType == transaction.TransactionType &&
                   PlayerId.Equals(transaction.PlayerId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, TransactionType, PlayerId);
        }
    }
}
