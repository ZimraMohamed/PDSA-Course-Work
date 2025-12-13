using System;
using System.Collections.Generic;
using System.Linq;

namespace PDSA.Core.Algorithms.SnakeAndLadder
{
    /// <summary>
    /// Solver class containing two algorithms to find the minimum number of dice throws
    /// required to reach the last cell in a Snake and Ladder game.
    /// </summary>
    public class SALSolver
    {
        /// <summary>
        /// Algorithm 1: BFS (Breadth-First Search)
        /// This is the optimal approach for finding the shortest path in an unweighted graph.
        /// Time Complexity: O(N) where N is the number of cells
        /// Space Complexity: O(N) for the queue and visited array
        /// 
        /// The algorithm treats each cell as a node in a graph and explores all possible moves
        /// level by level (dice throws 1-6), guaranteeing the shortest path.
        /// </summary>
        public static (int minimumThrows, List<int> path) SolveBFS(SALRequest request)
        {
            int boardSize = request.BoardSize;
            int totalCells = boardSize * boardSize;
            
            // Build the board with snakes and ladders
            int[] board = BuildBoard(request, totalCells);
            
            // BFS setup
            Queue<(int cell, int throws, List<int> path)> queue = new Queue<(int, int, List<int>)>();
            bool[] visited = new bool[totalCells + 1];
            
            // Start from cell 1 with 0 throws
            queue.Enqueue((1, 0, new List<int> { 1 }));
            visited[1] = true;
            
            while (queue.Count > 0)
            {
                var (currentCell, throwCount, currentPath) = queue.Dequeue();
                
                // Check if we reached the last cell
                if (currentCell == totalCells)
                {
                    return (throwCount, currentPath);
                }
                
                // Try all dice outcomes (1 to 6)
                for (int dice = 1; dice <= 6; dice++)
                {
                    int nextCell = currentCell + dice;
                    
                    // Check if next cell is within board
                    if (nextCell > totalCells)
                        continue;
                    
                    // Check for snake or ladder at next cell
                    int finalCell = board[nextCell];
                    
                    // If not visited, add to queue
                    if (!visited[finalCell])
                    {
                        visited[finalCell] = true;
                        var newPath = new List<int>(currentPath) { finalCell };
                        queue.Enqueue((finalCell, throwCount + 1, newPath));
                    }
                }
            }
            
            // If no path found (shouldn't happen in valid game)
            return (-1, new List<int>());
        }
        
        /// <summary>
        /// Algorithm 2: Dynamic Programming with Optimized BFS
        /// This approach uses dynamic programming principles to track minimum throws to each cell.
        /// Time Complexity: O(N) where N is the number of cells
        /// Space Complexity: O(N) for the dp array
        /// 
        /// Instead of using a visited array, we use a dp array to store minimum throws to reach each cell.
        /// This allows us to optimize and potentially process cells multiple times if a better path is found.
        /// </summary>
        public static (int minimumThrows, List<int> path) SolveDP(SALRequest request)
        {
            int boardSize = request.BoardSize;
            int totalCells = boardSize * boardSize;
            
            // Build the board with snakes and ladders
            int[] board = BuildBoard(request, totalCells);
            
            // DP array: dp[i] = minimum throws to reach cell i
            int[] dp = new int[totalCells + 1];
            int[] parent = new int[totalCells + 1]; // To reconstruct path
            
            // Initialize with infinity (unreachable)
            for (int i = 0; i <= totalCells; i++)
            {
                dp[i] = int.MaxValue;
                parent[i] = -1;
            }
            
            // Priority Queue (min-heap) based on throws count
            // Using SortedSet as a priority queue alternative
            var pq = new SortedSet<(int throws, int cell)>();
            
            // Start from cell 1 with 0 throws
            dp[1] = 0;
            pq.Add((0, 1));
            
            while (pq.Count > 0)
            {
                var current = pq.Min;
                pq.Remove(current);
                
                int throwCount = current.throws;
                int currentCell = current.cell;
                
                // If we found a better path already, skip
                if (throwCount > dp[currentCell])
                    continue;
                
                // If reached destination
                if (currentCell == totalCells)
                    break;
                
                // Try all dice outcomes (1 to 6)
                for (int dice = 1; dice <= 6; dice++)
                {
                    int nextCell = currentCell + dice;
                    
                    // Check if next cell is within board
                    if (nextCell > totalCells)
                        continue;
                    
                    // Check for snake or ladder at next cell
                    int finalCell = board[nextCell];
                    
                    // Calculate new throw count
                    int newThrows = throwCount + 1;
                    
                    // If this path is better, update
                    if (newThrows < dp[finalCell])
                    {
                        // Remove old entry if exists
                        if (dp[finalCell] != int.MaxValue)
                        {
                            pq.Remove((dp[finalCell], finalCell));
                        }
                        
                        dp[finalCell] = newThrows;
                        parent[finalCell] = currentCell;
                        pq.Add((newThrows, finalCell));
                    }
                }
            }
            
            // Reconstruct path
            List<int> path = new List<int>();
            if (dp[totalCells] != int.MaxValue)
            {
                int current = totalCells;
                while (current != -1)
                {
                    path.Insert(0, current);
                    current = parent[current];
                }
                
                return (dp[totalCells], path);
            }
            
            return (-1, new List<int>());
        }
        
        /// <summary>
        /// Helper method to build the board array with snakes and ladders.
        /// For each cell, board[i] represents where you actually end up if you land on cell i.
        /// If there's a snake or ladder at cell i, board[i] will be different from i.
        /// </summary>
        private static int[] BuildBoard(SALRequest request, int totalCells)
        {
            int[] board = new int[totalCells + 1];
            
            // Initially, each cell leads to itself
            for (int i = 0; i <= totalCells; i++)
            {
                board[i] = i;
            }
            
            // Add snakes (head -> tail)
            foreach (var snake in request.Snakes)
            {
                board[snake.Head] = snake.Tail;
            }
            
            // Add ladders (bottom -> top)
            foreach (var ladder in request.Ladders)
            {
                board[ladder.Bottom] = ladder.Top;
            }
            
            return board;
        }
        
        /// <summary>
        /// Validate that the game board is solvable and properly configured
        /// </summary>
        public static bool ValidateBoard(SALRequest request)
        {
            int totalCells = request.BoardSize * request.BoardSize;
            
            // Check board size is reasonable
            if (request.BoardSize < 2 || request.BoardSize > 20)
                return false;
            
            // Validate snakes
            foreach (var snake in request.Snakes)
            {
                // Head must be greater than tail
                if (snake.Head <= snake.Tail)
                    return false;
                
                // Both must be within board
                if (snake.Head <= 1 || snake.Head > totalCells || snake.Tail < 1 || snake.Tail >= totalCells)
                    return false;
                
                // Snake cannot end on last cell
                if (snake.Head == totalCells)
                    return false;
            }
            
            // Validate ladders
            foreach (var ladder in request.Ladders)
            {
                // Top must be greater than bottom
                if (ladder.Top <= ladder.Bottom)
                    return false;
                
                // Both must be within board
                if (ladder.Bottom < 1 || ladder.Bottom >= totalCells || ladder.Top <= 1 || ladder.Top > totalCells)
                    return false;
                
                // Ladder cannot start from last cell
                if (ladder.Bottom == totalCells)
                    return false;
            }
            
            // Check for overlapping snakes and ladders
            HashSet<int> startPositions = new HashSet<int>();
            
            foreach (var snake in request.Snakes)
            {
                if (!startPositions.Add(snake.Head))
                    return false; // Duplicate snake head
            }
            
            foreach (var ladder in request.Ladders)
            {
                if (!startPositions.Add(ladder.Bottom))
                    return false; // Duplicate ladder bottom or overlaps with snake
            }
            
            return true;
        }
    }
}
