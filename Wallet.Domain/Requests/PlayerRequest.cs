using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Requests
{
    public class PlayerRequest
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
}
