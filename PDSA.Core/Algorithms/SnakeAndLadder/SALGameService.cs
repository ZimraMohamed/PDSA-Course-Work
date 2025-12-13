using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PDSA.Core.Algorithms.SnakeAndLadder
{
    /// <summary>
    /// Service class that coordinates solving Snake and Ladder games using multiple algorithms
    /// and provides comprehensive results including execution times.
    /// </summary>
    public class SALGameService
    {
        /// <summary>
        /// Solve the Snake and Ladder game using both algorithms and return comprehensive results
        /// </summary>
        public SALSolution SolveGame(SALRequest request)
        {
            // Validate the board first
            if (!SALSolver.ValidateBoard(request))
            {
                throw new ArgumentException("Invalid board configuration");
            }
            
            var solution = new SALSolution
            {
                GameId = Guid.NewGuid().ToString(),
                AlgorithmResults = new List<SALAlgorithmResult>()
            };
            
            // Algorithm 1: BFS
            var bfsResult = RunAlgorithm(
                "BFS (Breadth-First Search)", 
                () => SALSolver.SolveBFS(request)
            );
            solution.AlgorithmResults.Add(bfsResult);
            
            // Algorithm 2: Dynamic Programming
            var dpResult = RunAlgorithm(
                "Dynamic Programming", 
                () => SALSolver.SolveDP(request)
            );
            solution.AlgorithmResults.Add(dpResult);
            
            // The minimum throws should be the same for both algorithms
            // (they both find the optimal solution)
            solution.MinimumThrows = bfsResult.MinimumThrows;
            
            // Verify both algorithms agree
            if (bfsResult.MinimumThrows != dpResult.MinimumThrows && 
                bfsResult.MinimumThrows != -1 && dpResult.MinimumThrows != -1)
            {
                throw new Exception("Algorithm mismatch: BFS and DP produced different results");
            }
            
            return solution;
        }
        
        /// <summary>
        /// Run a single algorithm and measure its execution time
        /// </summary>
        private SALAlgorithmResult RunAlgorithm(
            string algorithmName, 
            Func<(int minimumThrows, List<int> path)> algorithm)
        {
            var stopwatch = Stopwatch.StartNew();
            
            var (minimumThrows, path) = algorithm();
            
            stopwatch.Stop();
            
            return new SALAlgorithmResult
            {
                AlgorithmName = algorithmName,
                MinimumThrows = minimumThrows,
                ExecutionTimeMs = stopwatch.Elapsed.TotalMilliseconds,
                Path = path
            };
        }
        
        /// <summary>
        /// Generate a random Snake and Ladder board for testing
        /// </summary>
        public SALRequest GenerateRandomBoard(int boardSize, int numSnakes, int numLadders)
        {
            var random = new Random();
            int totalCells = boardSize * boardSize;
            var usedCells = new HashSet<int> { 1, totalCells }; // Reserve start and end
            
            var request = new SALRequest
            {
                BoardSize = boardSize,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };
            
            // Generate snakes
            for (int i = 0; i < numSnakes; i++)
            {
                int head, tail;
                int attempts = 0;
                do
                {
                    // Snake head should be in upper portion of board
                    head = random.Next(boardSize + 1, totalCells);
                    // Tail should be below head
                    tail = random.Next(1, head);
                    attempts++;
                } while ((usedCells.Contains(head) || usedCells.Contains(tail) || head == totalCells) && attempts < 100);
                
                if (attempts < 100)
                {
                    usedCells.Add(head);
                    usedCells.Add(tail);
                    request.Snakes.Add(new Snake { Head = head, Tail = tail });
                }
            }
            
            // Generate ladders
            for (int i = 0; i < numLadders; i++)
            {
                int bottom, top;
                int attempts = 0;
                do
                {
                    // Ladder bottom should be in lower portion
                    bottom = random.Next(1, totalCells - boardSize);
                    // Top should be above bottom
                    top = random.Next(bottom + 1, totalCells + 1);
                    attempts++;
                } while ((usedCells.Contains(bottom) || usedCells.Contains(top)) && attempts < 100);
                
                if (attempts < 100)
                {
                    usedCells.Add(bottom);
                    usedCells.Add(top);
                    request.Ladders.Add(new Ladder { Bottom = bottom, Top = top });
                }
            }
            
            return request;
        }
        
        /// <summary>
        /// Check if the user's answer is correct
        /// </summary>
        public bool CheckUserAnswer(int userAnswer, int correctAnswer)
        {
            return userAnswer == correctAnswer;
        }
        
        /// <summary>
        /// Get algorithm comparison details
        /// </summary>
        public string GetAlgorithmComparison(List<SALAlgorithmResult> results)
        {
            if (results.Count < 2)
                return "Insufficient algorithm results for comparison";
            
            var bfs = results[0];
            var dp = results[1];
            
            var faster = bfs.ExecutionTimeMs < dp.ExecutionTimeMs ? "BFS" : "Dynamic Programming";
            var timeDiff = Math.Abs(bfs.ExecutionTimeMs - dp.ExecutionTimeMs);
            
            return $"Both algorithms found the optimal solution of {bfs.MinimumThrows} throws. " +
                   $"{faster} was faster by {timeDiff:F4} ms. " +
                   $"BFS: {bfs.ExecutionTimeMs:F4} ms, DP: {dp.ExecutionTimeMs:F4} ms";
        }
    }
}
