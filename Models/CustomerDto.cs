using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace customers.Models
{
    public class CustomerDto
    {
        public long ID { get; set; }
        public int Score { get; set; }
        public int Rank { get; set; }
    }
}
