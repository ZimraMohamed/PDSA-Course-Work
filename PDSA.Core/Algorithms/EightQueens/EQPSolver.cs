using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PDSA.Core.Algorithms.EightQueens
{
    public static class EQPSolver
    {
        /// <summary>
        /// Find all solutions using backtracking (sequential).
        /// Each solution is represented as an int[] where index = row and value = column.
        /// </summary>
        public static (List<int[]> Solutions, long ExecutionMs) SolveSequential(int n = 8)
        {
            var sw = Stopwatch.StartNew();
            var solutions = new List<int[]>();
            var board = new int[n];
            SolveRow(0, n, board, solutions);
            sw.Stop();
            return (solutions, sw.ElapsedMilliseconds);
        }

        private static void SolveRow(int row, int n, int[] board, List<int[]> solutions)
        {
            if (row == n)
            {
                // record solution copy
                solutions.Add((int[])board.Clone());
                return;
            }

            for (int col = 0; col < n; col++)
            {
                if (IsSafe(board, row, col))
                {
                    board[row] = col;
                    SolveRow(row + 1, n, board, solutions);
                }
            }
        }

        private static bool IsSafe(int[] board, int row, int col)
        {
            for (int r = 0; r < row; r++)
            {
                int c = board[r];
                if (c == col) return false;
                if (Math.Abs(c - col) == Math.Abs(r - row)) return false;
            }
            return true;
        }

        /// <summary>
        /// Threaded variant: spawn tasks for first-row placements (or first two rows) to parallelize.
        /// Uses ConcurrentBag to collect solutions safely.
        /// </summary>
        public static (List<int[]> Solutions, long ExecutionMs) SolveThreaded(int n = 8)
        {
            var sw = Stopwatch.StartNew();
            var bag = new ConcurrentBag<int[]>();

            // Parallelize by first-row column choices (safe for n=8)
            var firstRowRange = Enumerable.Range(0, n);
            var tasks = firstRowRange.Select(col => Task.Run(() =>
            {
                var board = new int[n];
                board[0] = col;
                SolveRowParallel(1, n, board, bag);
            })).ToArray();

            Task.WaitAll(tasks);
            sw.Stop();
            return (bag.ToList(), sw.ElapsedMilliseconds);
        }

        private static void SolveRowParallel(int row, int n, int[] board, ConcurrentBag<int[]> bag)
        {
            if (row == n)
            {
                bag.Add((int[])board.Clone());
                return;
            }

            for (int col = 0; col < n; col++)
            {
                if (IsSafe(board, row, col))
                {
                    board[row] = col;
                    SolveRowParallel(row + 1, n, board, bag);
                }
            }
        }

        /// <summary>
        /// Validate if a given set of queen positions is a correct solution.
        /// Accepts list of {row, col} positions; returns true if exactly n queens and they don't attack.
        /// </summary>
        public static bool ValidateSolution(List<QueenPosition> positions, int n = 8)
        {
            if (positions == null) return false;
            if (positions.Count != n) return false;

            // check duplicate rows/cols
            if (positions.Select(p => p.Row).Distinct().Count() != n) return false;
            if (positions.Select(p => p.Col).Distinct().Count() != n) return false;

            var arr = new int[n];
            foreach (var p in positions)
            {
                if (p.Row < 0 || p.Row >= n || p.Col < 0 || p.Col >= n) return false;
                arr[p.Row] = p.Col;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (arr[i] == arr[j]) return false;
                    if (Math.Abs(arr[i] - arr[j]) == Math.Abs(i - j)) return false;
                }
            }
            return true;
        }
    }
}
