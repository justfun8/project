//中文注释跳表类
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// namespace customers.DataType
// {

//     public class SkipListNode
//     {
//         public long ID { get; }
//         public int Score { get; }
//         public SkipListNode? Backward { get; set; }
//         public SkipListNode[] Forward { get; }
//         public int[] Span { get; }

//         // span数组,记录节点到下一个节点的距离，如果在链表最后span是0

//         public SkipListNode(long id, int score, int maxLevel)
//         {
//             ID = id;
//             Score = score;
//             Forward = new SkipListNode[maxLevel];
//             Backward = null;
//             Span = new int[maxLevel];
//         }
//     }

//     public class SkipList_zh
//     {
//         const int MAX_LEVEL = 32;
//         const double P_FACTOR = 0.25;
//         private SkipListNode head;
//         private int level;
//         private Random random;
//         private Dictionary<long, int> IDScoreDictionary;

//         public SkipList()
//         {
//             head = new SkipListNode(-1, 0, MAX_LEVEL);
//             level = 1;
//             random = new Random();
//             IDScoreDictionary = new Dictionary<long, int>();
//             for (int i = 0; i < MAX_LEVEL; i++) // 初始化头节点的 Span 值为 0
//             {
//                 head.Span[i] = 0;
//             }
//         }

//         private int RandomLevel()
//         {
//             int lv = 1;
//             while (random.NextDouble() < P_FACTOR && lv < MAX_LEVEL)
//             {
//                 lv++;
//             }
//             return lv;
//         }
        


//         public int AddOrUpdate(long id, int score)
//         {
//             if (IDScoreDictionary.ContainsKey(id))
//             {
//                 int newScore = Update(id, score);
//                 return newScore;
//             }
//             else
//             {
//                 Add(id, score);
//                 return score;
//             }
//         }

//         public void Add(long id, int score)
//         {
//             SkipListNode[] update = new SkipListNode[MAX_LEVEL];
//             int[] rank = new int[MAX_LEVEL];
//             SkipListNode node = head;
//             //第一次for循环，记录插入点的位置update[],插入点的排名rank[]
//             for (int i = level - 1; i >= 0; i--)
//             {
//                 rank[i] = i == (level - 1) ? 0 : rank[i + 1]; //第一次进来rank是0，如果从上一层下来（i--）是上一层rank

//                 while (
//                     node.Forward[i] != null
//                     && (
//                         node.Forward[i].Score > score
//                         || (node.Forward[i].Score == score && node.Forward[i].ID < id)
//                     )
//                 )
//                 {
//                     rank[i] += node.Span[i];
//                     node = node.Forward[i];
//                 }
//                 update[i] = node;
//             } //
//             int newLevel = RandomLevel();
//             //随机生成的newlevel大于原来的层数，增加层数，设置插入点updat[]为头节点
//             if (newLevel > level)
//             {
//                 for (int i = level; i < newLevel; i++)
//                 {
//                     rank[i] = 0;
//                     update[i] = head;
//                     update[i].Span[i] = 0;
//                 }
//                 level = newLevel;
//             }

//             node = new SkipListNode(id, score, newLevel);
//             //第二次for循环，插入并且修改span
//             for (int i = 0; i < newLevel; ++i)
//             {
//                 node.Forward[i] = update[i].Forward[i];

//                 // 如果update[i]是该层的最后一个节点，则新节点的Span设置为0
//                 node.Span[i] =
//                     update[i].Forward[i] == null ? 0 : update[i].Span[i] - (rank[0] - rank[i]);

//                 update[i].Span[i] = rank[0] - rank[i] + 1;

//                 update[i].Forward[i] = node;
//             }

//             // 更新剩余 levels 的 span
//             for (int i = newLevel; i < level; i++)
//             {
//                 update[i].Span[i]++;
//             }
//             // 反向链接，仅在最底层设置
//             node.Backward = (update[0] == head) ? null : update[0];

//             // 如果最底层有Forward节点，则设置其Backward指向新节点node
//             if (node.Forward[0] != null)
//             {
//                 node.Forward[0].Backward = node;
//             }
//             IDScoreDictionary[id] = score;
//         }

//         public int Update(long id, int Score)
//         {
//             int newScore = IDScoreDictionary[id];
//             Remove(id);
//             newScore += Score;
//             Add(id, newScore);
//             return newScore;
//         }

//         public bool Remove(long id)
//         {
//             SkipListNode[] update = new SkipListNode[MAX_LEVEL];
//             SkipListNode curr = this.head;
//             int score = IDScoreDictionary[id];
//             int nodelevel = level; //记录被删除节点的层数+1
//             for (int i = level - 1; i >= 0; i--)
//             {
//                 // 在这一层中寻找小于或等于当前ID的最大节点
//                 while (
//                     curr.Forward[i] != null
//                     && (
//                         curr.Forward[i].Score > score
//                         || (curr.Forward[i].Score == score && curr.Forward[i].ID < id)
//                     )
//                 )
//                 {
//                     curr = curr.Forward[i];
//                 }
//                 if (curr.Forward[i] != null && curr.Forward[i].ID != id) //l
//                 {
//                     curr.Span[i]--; //删除后，比被删除节点层数高的最左边节点的span需要-1，图像上看即被删除节点上面的span需要-1
//                     nodelevel = i;
//                 }
//                 update[i] = curr;
//             }
//             curr = curr.Forward[0];

//             // 检查该节点是否为要删除的节点
//             if (curr == null || curr.ID != id)
//             {
//                 return false;
//             }

//             // 在各层更新相关指针，并修正span
//             // 更新forward指针和span值，并且处理backward指针
//             for (int i = 0; i < nodelevel; i++)
//             {
//                 // 先判断curr向前节点是否不存在，是则直接设span为0
//                 if (i < curr.Forward.Length && curr.Forward[i] == null)
//                 {
//                     update[i].Span[i] = 0;

//                     update[i].Forward[i] = null;
//                 }
//                 else if (i < curr.Forward.Length)
//                 {
//                     // 如果存在，按原来的计算方式累加span值
//                     update[i].Span[i] += curr.Span[i] - 1;
//                     update[i].Forward[i] = curr.Forward[i];
//                 }

//                 // 更新指针

//                 // 只在最底层更新backward指针
//                 if (i == 0 && curr.Forward[i] != null)
//                 {
//                     curr.Forward[i].Backward = update[i];
//                 }
//             }

//             // 更新跳表高度
//             while (level > 1 && head.Forward[level - 1] == null)
//             {
//                 level--;
//             }

//             // 从字典中移除该ID关联的分数
//             IDScoreDictionary.Remove(curr.ID);

//             return true;
//         }

//         // 方法二：获取指定排名范围内的客户
//         public List<(long ID, int Score, int rank)> GetCustomersByRange(int start, int end)
//         {
//             if (start > IDScoreDictionary.Count)
//             {
//                 throw new ArgumentOutOfRangeException(
//                     nameof(start),
//                     $"{start} Rank is out of bounds of the skip list."
//                 );
//             }
//             var result = new List<(long, int, int)>();
//             int currentRank = 0;
//             SkipListNode node = head;

//             // 找到第一个排名不低于start的节点
//             for (int i = level - 1; i >= 0; i--)
//             {
//                 while (node.Forward[i] != null && currentRank + node.Span[i] < start)
//                 {
//                     currentRank += node.Span[i];
//                     node = node.Forward[i];
//                 }
//             }
//             // 移动到实际的起始节点
//             currentRank++;
//             node = node.Forward[0];

//             // 收集排名在start和end之间的节点
//             while (node != null && currentRank <= end)
//             {
//                 result.Add((node.ID, node.Score, currentRank));
//                 currentRank++;
//                 node = node.Forward[0];
//             }

//             return result;
//         }

//         // 方法三：获取给定ID的客户周围的客户
//         //找到底层id节点
//         private (SkipListNode, int) FindNodeByScoreAndId(int score, long id)
//         {
//             SkipListNode node = head;
//             int rank = 0;
//             for (int i = level - 1; i >= 0; i--)
//             {
//                 while (
//                     node.Forward[i] != null
//                     && (
//                         node.Forward[i].Score > score
//                         || (node.Forward[i].Score == score && node.Forward[i].ID < id)
//                     )
//                 )
//                 {
//                     rank += (node.Span[i]); //累加跨越的节点数到rank
//                     node = node.Forward[i];
//                 }
//                 // 如果在任何层找到了目标ID，就停止循环
//                 if (node.Forward[i] != null && node.Forward[i].ID == id)
//                 {
//                     rank += node.Span[i];
//                     break;
//                 }
//             }
//             if (node.ID != id)
//             {
//                 node = node.Forward[0]; // 移动到下一个节点
//             }
//             //Console.WriteLine("ra:" + rank);

//             return (node, rank);
//         }

//         public (long ID, int Score, int Rank)[] GetCustomersAroundCustomer(
//             long id,
//             int high = 0,
//             int low = 0
//         )
//         {
//             // 检查ID是否存在
//             if (!IDScoreDictionary.ContainsKey(id))
//                 throw new KeyNotFoundException($"Customer with ID {id} not found.");

//             int score = IDScoreDictionary[id];

//             var (node, rank) = FindNodeByScoreAndId(score, id);
//             // if (node == null || node.ID != id)
//             //     return result; // 如果没有找到节点或者ID不匹配，则返回一个空数组
//             //当high太大，low太小，改变数组长度
//             if (high >= rank)
//                 high = rank - 1;
//             if (low + rank > IDScoreDictionary.Count)
//                 low = IDScoreDictionary.Count - rank;
//             var result = new (long ID, int Score, int Rank)[high + low + 1];

//             // 插入原节点
//             result[high] = (node.ID, node.Score, rank);

//             // 向前遍历high个节点并插入到数组
//             SkipListNode currentNode = node;
//             int irank = rank - 1;
//             for (int i = high - 1; i >= 0 && currentNode.Backward != null; i--, irank--)
//             {
//                 currentNode = currentNode.Backward;
//                 result[i] = (currentNode.ID, currentNode.Score, irank);
//             }

//             // 重置节点和排名
//             currentNode = node;
//             irank = rank + 1;

//             // 向后遍历low个节点并插入到数组（不包括中间的节点）
//             for (
//                 int i = high + 1;
//                 i < high + low + 1 && currentNode.Forward[0] != null;
//                 i++, irank++
//             )
//             {
//                 currentNode = currentNode.Forward[0];
//                 result[i] = (currentNode.ID, currentNode.Score, irank);
//             }

//             return result;
//         }
//     }
// }
