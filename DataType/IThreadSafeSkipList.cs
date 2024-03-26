using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using customers.Models;
namespace customers.DataType
{
    public interface IThreadSafeSkipList
    {
        int AddOrUpdate(long customerId, int score);
        List<CustomerDto> GetCustomersByRange(int? start, int? end);
        List<CustomerDto> GetCustomersAroundCustomerId(long customerId, int high, int low);

    }
}