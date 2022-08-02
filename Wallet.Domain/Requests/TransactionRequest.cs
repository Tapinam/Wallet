using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Enums;

namespace Wallet.Domain.Requests
{
    public class TransactionRequest
    {
        public decimal Amount { get; set; }
        public Guid PlayerId { get; set; }
        public TransactionType Type { get; set; }
    }
}
