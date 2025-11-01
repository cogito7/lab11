using System.Collections.Generic;
using UnityEngine;

public class PathfindingDynamically2 : MonoBehaviour
{
    private List<Vector2Int> path = new List<Vector2Int>();

    [Header("Grid Settings")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField, Range(0f, 1f)] private float obstacleProbability = 0.2f;

    [Header("Start & Goal Settings")]
    [SerializeField] private Vector2Int start = new Vector2Int(0, 0);
    [SerializeField] private Vector2Int goal = new Vector2Int(9, 9);
    [SerializeField] private bool randomizeStartAndGoal = false;

    private int[,] grid;
    private Vector2Int[] directions =
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    private void Start()
    {
        EnsureStartAndGoalUnique();
        GenerateValidGrid(width, height, obstacleProbability);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int randomPos;
            do
            {
                randomPos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            } while (grid[randomPos.y, randomPos.x] == 1 || randomPos == start || randomPos == goal);

            AddObstacleAndEnsureSolvable(randomPos);
        }

        if (Input.GetMouseButtonDown(1))
        {
            GenerateValidGrid(width, height, obstacleProbability);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            RandomizeStartOrGoal(ref start);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            RandomizeStartOrGoal(ref goal);
        }
    }

    private void EnsureStartAndGoalUnique()
    {
        if (start == goal)
        {
            // Move goal one step away if it overlaps with start
            goal.x = (goal.x + 1) % width;
            goal.y = (goal.y + 1) % height;
        }
    }

    private void RandomizeStartOrGoal(ref Vector2Int point)
    {
        Vector2Int oldPoint = point;
        for (int attempt = 0; attempt < 100; attempt++)
        {
            point = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

            // Make sure it does not overlap the other point
            if (grid[point.y, point.x] == 1 || point == (point == start ? goal : start))
                continue;

            FindPath(start, goal);
            if (path.Count > 0)
                return;
        }
        point = oldPoint;
    }

    private void AddObstacleAndEnsureSolvable(Vector2Int position)
    {
        int oldValue = grid[position.y, position.x];
        grid[position.y, position.x] = 1;

        FindPath(start, goal);

        if (path.Count == 0)
        {
            grid[position.y, position.x] = oldValue;
            FindPath(start, goal);
        }
    }

    public void GenerateRandomGrid(int width, int height, float obstacleProbability)
    {
        grid = new int[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[y, x] = Random.value < obstacleProbability ? 1 : 0;
            }
        }

        if (randomizeStartAndGoal)
        {
            do
            {
                start = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            } while (grid[start.y, start.x] == 1);

            do
            {
                goal = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            } while (grid[goal.y, goal.x] == 1 || goal == start);
        }

        EnsureStartAndGoalUnique();
        grid[start.y, start.x] = 0;
        grid[goal.y, goal.x] = 0;
    }

    public void GenerateValidGrid(int width, int height, float obstacleProbability, int maxAttempts = 100)
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            GenerateRandomGrid(width, height, obstacleProbability);
            FindPath(start, goal);
            if (path.Count > 0)
                return;
        }
        Debug.LogWarning("Failed to generate a solvable grid after " + maxAttempts + " attempts.");
    }

    private void FindPath(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);

        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            if (current == goal)
                break;

            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current + direction;
                if (!IsInBounds(next) || grid[next.y, next.x] == 1 || cameFrom.ContainsKey(next))
                    continue;

                frontier.Enqueue(next);
                cameFrom[next] = current;
            }
        }

        path.Clear();

        if (!cameFrom.ContainsKey(goal))
        {
            Debug.Log("Path not found!");
            return;
        }

        Vector2Int step = goal;
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Add(start);
        path.Reverse();
    }

    private bool IsInBounds(Vector2Int point)
    {
        return point.x >= 0 && point.x < grid.GetLength(1) &&
               point.y >= 0 && point.y < grid.GetLength(0);
    }

    private void OnDrawGizmos()
    {
        if (grid == null) return;
        float cellSize = 1f;

        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3 cellPos = new Vector3(x * cellSize, 0, y * cellSize);
                Gizmos.color = grid[y, x] == 1 ? Color.black : Color.white;
                Gizmos.DrawCube(cellPos, new Vector3(cellSize, 0.1f, cellSize));
            }
        }

        Gizmos.color = Color.blue;
        foreach (var step in path)
        {
            Vector3 cellPos = new Vector3(step.x * cellSize, 0, step.y * cellSize);
            Gizmos.DrawCube(cellPos, new Vector3(cellSize, 0.1f, cellSize));
        }

        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(start.x * cellSize, 0, start.y * cellSize),
                        new Vector3(cellSize, 0.1f, cellSize));

        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(goal.x * cellSize, 0, goal.y * cellSize),
                        new Vector3(cellSize, 0.1f, cellSize));
    }
}
