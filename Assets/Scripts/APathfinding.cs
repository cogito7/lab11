using System.Collections.Generic;
using UnityEngine;

// Simple binary-heap min-priority queue for Unity (.NET Standard compatible)
internal class PriorityQueue<T>
{
    private struct Node
    {
        public T item;
        public int priority;
        public Node(T i, int p) { item = i; priority = p; }
    }

    private List<Node> heap = new List<Node>();

    public int Count => heap.Count;

    public void Enqueue(T item, int priority)
    {
        heap.Add(new Node(item, priority));
        int i = heap.Count - 1;
        // Bubble up
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (heap[i].priority >= heap[parent].priority) break;
            var tmp = heap[i];
            heap[i] = heap[parent];
            heap[parent] = tmp;
            i = parent;
        }
    }

    public T Dequeue()
    {
        if (heap.Count == 0) throw new System.InvalidOperationException("PriorityQueue is empty");
        T ret = heap[0].item;
        int last = heap.Count - 1;
        heap[0] = heap[last];
        heap.RemoveAt(last);
        // Bubble down
        int idx = 0;
        while (true)
        {
            int left = 2 * idx + 1;
            int right = left + 1;
            int smallest = idx;

            if (left < heap.Count && heap[left].priority < heap[smallest].priority)
                smallest = left;
            if (right < heap.Count && heap[right].priority < heap[smallest].priority)
                smallest = right;

            if (smallest == idx) break;
            var tmp = heap[idx];
            heap[idx] = heap[smallest];
            heap[smallest] = tmp;
            idx = smallest;
        }
        return ret;
    }
}

// A* pathfinding MonoBehaviour using the custom PriorityQueue
public class APathfinding : MonoBehaviour
{
    private List<Vector2Int> path = new List<Vector2Int>();
    private Vector2Int start = new Vector2Int(0, 1);
    private Vector2Int goal = new Vector2Int(4, 4);
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };
    private int[,] grid = new int[,]
    {
        { 0, 1, 0, 0, 0 },
        { 0, 1, 0, 1, 0 },
        { 0, 0, 0, 1, 0 },
        { 0, 1, 1, 1, 0 },
        { 0, 0, 0, 0, 0 }
    };

    private void Start()
    {
        FindPath(start, goal);
    }

    private void OnDrawGizmos()
    {
        float cellSize = 1f;
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, 0, y * cellSize);
                Gizmos.color = grid[y, x] == 1 ? Color.black : Color.white;
                Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
            }
        }
        foreach (var step in path)
        {
            Vector3 cellPosition = new Vector3(step.x * cellSize, 0, step.y * cellSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
        }
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(start.x * cellSize, 0, start.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));
        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(goal.x * cellSize, 0, goal.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));
    }

    private bool IsInBounds(Vector2Int point)
    {
        return point.x >= 0 && point.x < grid.GetLength(1) && point.y >= 0 && point.y < grid.GetLength(0);
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private void FindPath(Vector2Int start, Vector2Int goal)
    {
        var frontier = new PriorityQueue<Vector2Int>();
        frontier.Enqueue(start, 0);
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var costSoFar = new Dictionary<Vector2Int, int>();
        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            if (current == goal) break;

            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current + direction;
                if (!IsInBounds(next) || grid[next.y, next.x] == 1)
                    continue;
                int newCost = costSoFar[current] + 1;
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    int priority = newCost + Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))
        {
            Debug.Log("Path not found.");
            path.Clear();
            return;
        }

        path.Clear();
        Vector2Int step = goal;
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Add(start);
        path.Reverse();
    }
}