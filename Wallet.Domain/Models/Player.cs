using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public PlayerWallet Wallet { get; set; }
    }
}
