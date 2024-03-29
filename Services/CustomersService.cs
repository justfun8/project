using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using customers.DataType;
using customers.Models;

namespace customers.Services;

public class CustomersService : ICustomersService
{
    private readonly ThreadSafeSkipList _skipList;

    public CustomersService(ThreadSafeSkipList skipList)
    {
        _skipList = skipList;
    }

    public int AddOrUpdate(long customerId, int score)
    {
        if (score < -1000 || score > 1000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(score),
                "Score must be within ±1000 range."
            );
        }

        if (customerId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(customerId),
                "customerId must be positive."
            );
        }

        return _skipList.AddOrUpdate(customerId, score);
    }

    public List<CustomerDto> GetCustomersByRange(int? start, int? end)
    {
        if (!start.HasValue || !end.HasValue)
        {
            throw new ArgumentOutOfRangeException(nameof(start), "start and end need value.");
        }

        if (start.Value > end.Value)
        {
            // throw new ArgumentOutOfRangeException(
            //     nameof(end),
            //     start.Value,
            //     $"The 'end' index cannot be less than {start.Value}."
            // );
            return new List<CustomerDto>();
        }
        if (start.Value <= 0)
        {
            // throw new ArgumentOutOfRangeException(nameof(start), "start must be positive.");
            return new List<CustomerDto>();
        }
        var customerDtos = _skipList.GetCustomersByRange(start.Value, end.Value);
        // var customerDtos = customersTupleList
        // .Select(c => new CustomerDto
        // {
        //     CustomerId = c.ID,
        //     Score = c.Score,
        //     Rank = c.rank
        // })
        // .ToList();

        return customerDtos;
    }

    public List<CustomerDto> GetCustomersAroundCustomerId(long customerId, int high, int low)
    {
        // if (high < 0 || low < 0)
        // {
        //     throw new ArgumentOutOfRangeException("high and low must be positive.");
        // }
        if(high<0)high=0;
        if(low<0)low=0;

        var customerDtos = _skipList.GetCustomersAroundCustomerId(customerId, high, low);
        // var customerDtos = resultsTuple
        //     .Select(tuple => new CustomerDto
        //     {
        //         CustomerId = tuple.ID,
        //         Score = tuple.Score,
        //         Rank = tuple.Rank
        //     })
        //     .ToList();
        return customerDtos;
    }
}


// public class CustomerNotFoundException : Exception
// {
//     public CustomerNotFoundException(string message)
//         : base(message) { }
// }
