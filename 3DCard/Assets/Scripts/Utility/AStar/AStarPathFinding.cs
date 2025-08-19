using System;
using System.Collections.Generic;
using UnityEngine;


public class AStarNode
{
    public Vector2Int coor;
    public AStarNode parentNode; // 父节点的引用
    public int gCost; // 从起点到该节点的实际代价
    public int hCost; // 从该节点到终点的启发式代价
    public int fCost => gCost + hCost; // 总代价

    public AStarNode(Vector2Int _coor, int _gCost, int _hCost, AStarNode _parentNode)
    {
        coor = _coor;
        gCost = _gCost;
        hCost = _hCost;
        parentNode = _parentNode;
    }
}
public static class AStarPathFinding
{
    // 假设这是包含所有障碍物的集合
    private static readonly HashSet<Vector2Int> allUnwalkableCoor = MapMgr.Instance.allUnwalkableCoor;

    private static int GetHeuristic(Vector2Int a, Vector2Int b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }

    private static List<Vector2Int> GetNeighbors(Vector2Int position)
    {
        var neighbors = new List<Vector2Int>();
        neighbors.Add(position + Vector2Int.up);
        neighbors.Add(position + Vector2Int.down);
        neighbors.Add(position + Vector2Int.left);
        neighbors.Add(position + Vector2Int.right);
        return neighbors;
    }
    public static List<Vector2Int> FindPath(Vector2Int startCoor, Vector2Int endCoor)
    {
        if (allUnwalkableCoor.Contains(startCoor) || allUnwalkableCoor.Contains(endCoor))
        {
            return null;
        }

        var openList = new PriorityQueue<AStarNode, int>();
        var closedList = new HashSet<Vector2Int>();

        // 增加一个字典来记录每个位置的 gCost，避免重复入队
        var gCostMap = new Dictionary<Vector2Int, int>();

        var startNode = new AStarNode(startCoor, 0, GetHeuristic(startCoor, endCoor), null);
        openList.Enqueue(startNode, startNode.fCost);
        gCostMap[startCoor] = 0;

        while (openList.Count > 0)
        {
            var currentNode = openList.Dequeue();
            var currentPos = currentNode.coor;

            // 核心优化: 检查该节点是否已经被更好的路径发现过
            if (gCostMap.ContainsKey(currentPos) && currentNode.gCost > gCostMap[currentPos])
            {
                continue;
            }

            closedList.Add(currentPos);

            if (currentPos == endCoor)
            {
                return ReconstructPath(currentNode);
            }

            foreach (var neighborPos in GetNeighbors(currentPos))
            {
                if (allUnwalkableCoor.Contains(neighborPos) || closedList.Contains(neighborPos))
                {
                    continue;
                }

                int newGCost = currentNode.gCost + 1;

                // 如果新路径更优，才进行处理
                if (!gCostMap.ContainsKey(neighborPos) || newGCost < gCostMap[neighborPos])
                {
                    gCostMap[neighborPos] = newGCost;

                    var neighborNode = new AStarNode(neighborPos, newGCost, GetHeuristic(neighborPos, endCoor), currentNode);
                    openList.Enqueue(neighborNode, neighborNode.fCost);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 使用父节点引用回溯路径
    /// </summary>
    private static List<Vector2Int> ReconstructPath(AStarNode endNode)
    {
        var path = new List<Vector2Int>();
        var currentNode = endNode;
        while (currentNode != null)
        {
            path.Add(currentNode.coor);
            currentNode = currentNode.parentNode;
        }
        path.Reverse(); // 反转列表以获得从起点到终点的顺序
        return path;
    }

    public static Vector2Int FindPathNextStep(Vector2Int startCoor, Vector2Int endCoor)
    {
        var path = FindPath(startCoor, endCoor);
        if (path != null && path.Count > 1)
        {
            return path[1];
        }
        return startCoor;
    }
}


public class PriorityQueue<TElement, TPriority>
    where TPriority : IComparable<TPriority>
{
    private readonly List<(TElement Element, TPriority Priority)> _heap;

    public PriorityQueue()
    {
        _heap = new List<(TElement Element, TPriority Priority)>();
    }

    public int Count => _heap.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        _heap.Add((element, priority));
        SiftUp(_heap.Count - 1);
    }

    public TElement Dequeue()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("The queue is empty.");
        }

        var result = _heap[0].Element;
        var last = _heap.Count - 1;
        _heap[0] = _heap[last];
        _heap.RemoveAt(last);

        if (Count > 0)
        {
            SiftDown(0);
        }

        return result;
    }

    public TElement Peek()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("The queue is empty.");
        }
        return _heap[0].Element;
    }

    private void SiftUp(int index)
    {
        var item = _heap[index];
        var parentIndex = (index - 1) / 2;

        while (index > 0 && item.Priority.CompareTo(_heap[parentIndex].Priority) < 0)
        {
            _heap[index] = _heap[parentIndex];
            index = parentIndex;
            parentIndex = (index - 1) / 2;
        }

        _heap[index] = item;
    }

    private void SiftDown(int index)
    {
        var item = _heap[index];
        var leftChildIndex = 2 * index + 1;

        while (leftChildIndex < _heap.Count)
        {
            var rightChildIndex = leftChildIndex + 1;
            var minChildIndex = leftChildIndex;

            if (rightChildIndex < _heap.Count && _heap[rightChildIndex].Priority.CompareTo(_heap[leftChildIndex].Priority) < 0)
            {
                minChildIndex = rightChildIndex;
            }

            if (item.Priority.CompareTo(_heap[minChildIndex].Priority) <= 0)
            {
                break;
            }

            _heap[index] = _heap[minChildIndex];
            index = minChildIndex;
            leftChildIndex = 2 * index + 1;
        }

        _heap[index] = item;
    }
}