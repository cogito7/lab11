Lab 11
=====

> Melanie Galindo Waugh

Each pathfinding algorithm calculates and prioritizes paths differently. Pathfinding (Breadth First Search) explores the grid layer by layer starting at the start node. It looks at neighboring nodes to create the path. BFS uses the queue (FIFO). DPathfinding (Dijkstra's Algorithm) introduces a priority queue. Each node has a cost depending on the distance from the starting node. Nodes with the lowest cost are explored first. It uses a priority queue repeatedly picking the least costly node to expand. APathfinding (A* Algorithm) is similar to Dijkstra but calculates the estimate of how close the current node is to the goal. This is referred to as a heuristic, also the Manhattan Distance.

Some challenges arose when dynamically updating obstacles in real time using the BFS algorithm. When an obstacle was added or removed, it could block parts of the path, making the maze unsolvable. I also encountered issues where the start and goal nodes sometimes overlapped. My solution was to create checks ensuring that the start and goal never occupy the same cell, and that obstacles only spawn if the maze remains solvable. These checks ensured that a valid path always exists.

The algorithm I would choose if I needed to adapt it for larger grids or an open-world setting would be A*. BFS does not focus its search toward the goal, so while it worked well in this small maze, it could become inefficient in larger or more dynamic environments with more moving parts, leading to significant frame rate drops.

If I were to add weighted cells (e.g., "difficult terrain" areas), I would prefer to use A* because I could simply modify the weighted cost in the A*'s cost + heuristic formula. For example, if we define the following:

    float g = costSoFar[current]; //actual cost so far
    float h = Vector2Int.Distance(current, goal); //heuristic estimate
    float f = g + h; //total estimated cost of the path

I can expand on g to include additional weights depending on the type and diffuliculty level of a particular terrain.


