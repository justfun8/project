using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using customers.Models;

namespace customers.Services
{
    public interface ICustomersService
    {
        int AddOrUpdateCustomerScore(long customerId, int score);
        List<CustomerDto> GetCustomersByRange(int? start, int? end);
        List<CustomerDto> GetCustomersAroundCustomerId(int customerId, int high, int low);
        
    }
}