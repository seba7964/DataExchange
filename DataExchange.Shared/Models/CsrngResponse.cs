using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExchange.Shared.Models
{
    public class CsrngResponse
    {
        public string Status { get; set; } = string.Empty;
        public int Min { get; set; }
        public int Max { get; set; }
        public int Random { get; set; }
    }
}
