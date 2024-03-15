using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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


        public List<(long ID, int Score, int rank)> GetCustomersByRange(int start, int end)
        {
            _rwLock.EnterReadLock();
            try
            {
                List<(long ID, int Score, int Rank)> rangeCustomers = skipList.GetCustomersByRange(
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

        public (long ID, int Score, int Rank)[] GetCustomersAroundCustomer(
            long id,
            int high,
            int low
        )
        {
            _rwLock.EnterReadLock();
            try
            {
                (long ID, int Score, int Rank)[] Customers = skipList.GetCustomersAroundCustomer(
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