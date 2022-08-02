using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Responses
{
    public class ResponseWrapper<T>
    {
        public T Payload { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}
