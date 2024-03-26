using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using customers.Models;

namespace customers.DataType
{
    public class ThreadSafeSkipList
    {
        private ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        SkipList skipList = new SkipList();

        public int AddOrUpdate(long id, int score)
        {
            _rwLock.EnterWriteLock();
            try
            {
                int newScore=skipList.AddOrUpdate(id, score);
                return newScore;
            }
            finally
            {
                _rwLock.ExitWriteLock();
                
            }
        }


        public List<CustomerDto> GetCustomersByRange(int? start, int? end)
        {
            _rwLock.EnterReadLock();
            try
            {
                List<CustomerDto> rangeCustomers = skipList.GetCustomersByRange(
                    start,
                    end
                );
                return rangeCustomers; 
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        public List<CustomerDto> GetCustomersAroundCustomerId(
            long id,
            int high,
            int low
        )
        {
            _rwLock.EnterReadLock();
            try
            {
                List<CustomerDto> Customers = skipList.GetCustomersAroundCustomerId(
                    id,
                    high,
                    low
                );
                return Customers; 
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

    }
}