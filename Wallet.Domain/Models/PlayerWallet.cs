using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Models
{
    public class PlayerWallet
    {
        public PlayerWallet()
        {
            Ballance = 0;
        }

        public decimal Ballance { get; set; }
    }
}
