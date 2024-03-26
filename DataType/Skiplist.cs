using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using customers.Models;

namespace customers.DataType
{
    public class SkipListNode
    {
        public long ID { get; }
        public int Score { get; }
        public SkipListNode? Backward { get; set; }
        public SkipListNode[] Forward { get; }
        public int[] Span { get; }

        // Span array, records the distance to the next node; if at the end of the list, the span is 0
        public SkipListNode(long id, int score, int maxLevel)
        {
            ID = id;
            Score = score;
            Forward = new SkipListNode[maxLevel];
            Backward = null;
            Span = new int[maxLevel];
        }
    }

    public class SkipList
    {
        const int MAX_LEVEL = 32;
        const double P_FACTOR = 0.25;
        private SkipListNode head;
        private int level;
        private Random random;
        private Dictionary<long, int> IDScoreDictionary;

        public SkipList()
        {
            head = new SkipListNode(-1, 0, MAX_LEVEL);
            level = 1;
            random = new Random();
            IDScoreDictionary = new Dictionary<long, int>();
            for (int i = 0; i < MAX_LEVEL; i++) // Initialize the head node's span values to 0
            {
                head.Span[i] = 0;
            }
        }

        private int RandomLevel()
        {
            int lv = 1;
            while (random.NextDouble() < P_FACTOR && lv < MAX_LEVEL)
            {
                lv++;
            }
            return lv;
        }

        /// <summary>
        /// method 1: addorupdate
        /// </summary>
        public int AddOrUpdate(long id, int score)
        {
            if (IDScoreDictionary.ContainsKey(id))
            {
                int newScore = Update(id, score);
                return newScore;
            }
            else
            {
                Add(id, score);
                return score;
            }
        }

        public void Add(long id, int score)
        {
            SkipListNode[] update = new SkipListNode[MAX_LEVEL];
            int[] rank = new int[MAX_LEVEL];
            SkipListNode node = head;
            // First for-loop: record the insert position update[] and the rank of insertion point rank[]
            for (int i = level - 1; i >= 0; i--)
            {
                rank[i] = i == (level - 1) ? 0 : rank[i + 1];
                // On first entry, rank is 0, and if coming down from above (i--), it's the rank of the previous level
                while (
                    node.Forward[i] != null
                    && (
                        node.Forward[i].Score > score
                        || (node.Forward[i].Score == score && node.Forward[i].ID < id)
                    )
                )
                {
                    rank[i] += node.Span[i];
                    node = node.Forward[i];
                }
                update[i] = node;
            } //
            int newLevel = RandomLevel();
            // If the randomly generated newLevel is higher than the current number of levels
            if (newLevel > level)
            {
                for (int i = level; i < newLevel; i++)
                {
                    rank[i] = 0;
                    update[i] = head;
                    update[i].Span[i] = 0;
                }
                level = newLevel;
            }

            node = new SkipListNode(id, score, newLevel);
            // The second for-loop: insert and modify the span
            for (int i = 0; i < newLevel; ++i)
            {
                node.Forward[i] = update[i].Forward[i];

                // If update[i] is the last node of that level, then set the new node's Span to 0
                node.Span[i] =
                    update[i].Forward[i] == null ? 0 : update[i].Span[i] - (rank[0] - rank[i]);

                update[i].Span[i] = rank[0] - rank[i] + 1;

                update[i].Forward[i] = node;
            }

            // Update the spans of the remaining levels

            for (int i = newLevel; i < level; i++)
            {
                update[i].Span[i]++;
            }
            // Set backward , only on the lowest level
            node.Backward = (update[0] == head) ? null : update[0];
            // If there is a forward node on the lowest level, then set its backward to point to the new node
            if (node.Forward[0] != null)
            {
                node.Forward[0].Backward = node;
            }
            IDScoreDictionary[id] = score;
        }

        public int Update(long id, int Score)
        {
            int newScore = IDScoreDictionary[id];
            Remove(id);
            newScore += Score;
            Add(id, newScore);
            return newScore;
        }

        public bool Remove(long id)
        {
            SkipListNode[] update = new SkipListNode[MAX_LEVEL];
            SkipListNode curr = this.head;
            int score = IDScoreDictionary[id];
            int nodelevel = level; //remove node's level+1
            for (int i = level - 1; i >= 0; i--)
            {
                // Find the left node of the deleted node
                while (
                    curr.Forward[i] != null
                    && (
                        curr.Forward[i].Score > score
                        || (curr.Forward[i].Score == score && curr.Forward[i].ID < id)
                    )
                )
                {
                    curr = curr.Forward[i];
                }
                if (curr.Forward[i] != null && curr.Forward[i].ID != id) //l
                {
                    curr.Span[i]--;
                    // After deletion, subtract 1 from the span of the leftmost node at levels above the deleted node; visually, this means reducing the span above the deleted node by 1.

                    nodelevel = i;
                }
                update[i] = curr;
            }
            curr = curr.Forward[0];

            // check id of node
            if (curr == null || curr.ID != id)
            {
                return false;
            }

            // upload forward,span,backward
            for (int i = 0; i < nodelevel; i++)
            {
                // First determine whether Curr forward node exists, then directly set span to 0
                if (i < curr.Forward.Length && curr.Forward[i] == null)
                {
                    update[i].Span[i] = 0;

                    update[i].Forward[i] = null;
                }
                else if (i < curr.Forward.Length)
                {
                    update[i].Span[i] += curr.Span[i] - 1;
                    update[i].Forward[i] = curr.Forward[i];
                }

                // upload backward at first level
                if (i == 0 && curr.Forward[i] != null)
                {
                    curr.Forward[i].Backward = update[i];
                }
            }

            // uplaod level
            while (level > 1 && head.Forward[level - 1] == null)
            {
                level--;
            }

            IDScoreDictionary.Remove(curr.ID);

            return true;
        }

        /// <summary>
        ///  method 2: obtain customers within a specified ranking range
        /// </summary>
        // public List<(long ID, int Score, int rank)> GetCustomersByRange(int? start, int? end)
        public List<CustomerDto> GetCustomersByRange(int? start, int? end)
        {
            if (start > IDScoreDictionary.Count)
            {
                // throw new ArgumentOutOfRangeException(
                //     nameof(start),
                //     $"{start} Rank is out of bounds of the skip list."
                // );
                return new List<CustomerDto>();
            }
            // var result = new List<(long, int, int)>();
            var result = new List<CustomerDto>();
            int currentRank = 0;
            SkipListNode node = head;
            // Find the first node not ranked lower than start
            for (int i = level - 1; i >= 0; i--)
            {
                while (node.Forward[i] != null && currentRank + node.Span[i] < start)
                {
                    currentRank += node.Span[i];
                    node = node.Forward[i];
                }
            }
            // Move to the actual starting node
            node = node.Forward[0];
            currentRank++;
            // while (node != null && currentRank <= end)
            // {
            //     result.Add((node.ID, node.Score, currentRank));
            //     currentRank++;
            //     node = node.Forward[0];
            // }
            while (node != null && currentRank <= end)
            {
                result.Add(
                    new CustomerDto
                    {
                        CustomerId = node.ID,
                        Score = node.Score,
                        Rank = currentRank
                    }
                );
                node = node.Forward[0];
                currentRank++;
            }

            return result;
        }

        /// <summary>
        /// Method 3: Get the neighboring customers for a given customer ID
        /// </summary>
        // Locate the bottom-level node with the specified ID

        private (SkipListNode, int) FindNodeByScoreAndId(int score, long id)
        {
            SkipListNode node = head;
            int rank = 0;
            for (int i = level - 1; i >= 0; i--)
            {
                while (
                    node.Forward[i] != null
                    && (
                        node.Forward[i].Score > score
                        || (node.Forward[i].Score == score && node.Forward[i].ID < id)
                    )
                )
                {
                    rank += (node.Span[i]); //add span to rank
                    node = node.Forward[i];
                }
                // if find id, stop for loop
                if (node.Forward[i] != null && node.Forward[i].ID == id)
                {
                    rank += node.Span[i];
                    break;
                }
            }
            if (node.ID != id)
            {
                node = node.Forward[0];
            }

            return (node, rank);
        }

        public List<CustomerDto> GetCustomersAroundCustomerId(long id, int high = 0, int low = 0)
        {
            if (!IDScoreDictionary.ContainsKey(id))
                throw new KeyNotFoundException($"Customer with ID {id} not found.");

            int score = IDScoreDictionary[id];

            var (node, rank) = FindNodeByScoreAndId(score, id);
            var result = new List<CustomerDto>();
            result.Add(
                new CustomerDto
                {
                    CustomerId = node.ID,
                    Score = node.Score,
                    Rank = rank
                }
            );
            // If the high value is too large, or the low value too small, adjust the array length
            // if (high >= rank)
            //     high = rank - 1;
            // if (low + rank > IDScoreDictionary.Count)
            //     low = IDScoreDictionary.Count - rank;
            // var result = new (long ID, int Score, int Rank)[high + low + 1];

            // Insert the original node

            // Insert the original node's data

            SkipListNode currentNode = node;
            int currentRank = rank - 1;

            // Gather nodes with higher rank
            // for (int i = high - 1; i >= 0 && currentNode.Backward != null; i--, currentRank--)
            for (int i = 0; i < high && node.Backward != null; i++, currentRank--)
            {
                currentNode = currentNode.Backward;
                result.Add(
                    new CustomerDto
                    {
                        CustomerId = currentNode.ID,
                        Score = currentNode.Score,
                        Rank = currentRank
                    }
                );
            }
            result.Reverse();

            currentNode = node;
            currentRank = rank + 1;

            // Gather nodes with lower rank
            // for (
            //     int i = high + 1;
            //     i < high + low + 1 && currentNode.Forward[0] != null;
            //     i++, currentRank++
            // )
            for (int i = 1; i <= low && node.Forward[0] != null; i++, currentRank++)
            {
                currentNode = currentNode.Forward[0];
                result.Add(
                    new CustomerDto
                    {
                        CustomerId = currentNode.ID,
                        Score = currentNode.Score,
                        Rank = currentRank
                    }
                );
            }
            // result[high] = (node.ID, node.Score, rank);

            // SkipListNode currentNode = node;
            // int irank = rank - 1;
            // for (int i = high - 1; i >= 0 && currentNode.Backward != null; i--, irank--)
            // {
            //     currentNode = currentNode.Backward;
            //     result[i] = (currentNode.ID, currentNode.Score, irank);
            // }

            // currentNode = node;
            // irank = rank + 1;

            // for (
            //     int i = high + 1;
            //     i < high + low + 1 && currentNode.Forward[0] != null;
            //     i++, irank++
            // )
            // {
            //     currentNode = currentNode.Forward[0];
            //     result[i] = (currentNode.ID, currentNode.Score, irank);
            // }

            return result;
        }
    }
}
