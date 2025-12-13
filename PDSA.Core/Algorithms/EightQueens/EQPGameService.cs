using System;
using System.Collections.Generic;
using System.Linq;

namespace PDSA.Core.Algorithms.EightQueens
{
    public static class EQPGameService
    {
        // In-memory simple storage for demo. Replace with DB integration as needed.
        private static readonly Dictionary<Guid, EQPGameRound> rounds = new();
        private static readonly HashSet<string> recordedSolutions = new(); // store solution signatures
        private static readonly List<(Guid GameId, string PlayerName, List<QueenPosition> Positions)> savedPlayerSolutions = new();

        public static EQPGameRound CreateNewGameRound(int boardSize = 8)
        {
            var round = new EQPGameRound { GameId = Guid.NewGuid(), BoardSize = boardSize };
            rounds[round.GameId] = round;
            return round;
        }

        public static EQPGameResult SolveAll(Guid gameId)
        {
            if (!rounds.ContainsKey(gameId))
            {
                return new EQPGameResult { Success = false, ErrorMessage = "Game not found." };
            }

            try
            {
                var round = rounds[gameId];
                // Sequential
                var (seqSolutions, seqMs) = EQPSolver.SolveSequential(round.BoardSize);
                // Threaded
                var (thrSolutions, thrMs) = EQPSolver.SolveThreaded(round.BoardSize);

                var results = new List<EQPAlgorithmResult>
                {
                    new EQPAlgorithmResult { AlgorithmName = "Sequential Backtracking", SolutionsFound = seqSolutions.Count, ExecutionTimeMs = seqMs },
                    new EQPAlgorithmResult { AlgorithmName = "Threaded Backtracking", SolutionsFound = thrSolutions.Count, ExecutionTimeMs = thrMs }
                };

                // Optionally record canonical solution signatures in memory
                foreach (var sol in seqSolutions)
                {
                    recordedSolutions.Add(Signature(sol));
                }

                return new EQPGameResult
                {
                    Round = round,
                    SequentialSolutionsCount = seqSolutions.Count,
                    ThreadedSolutionsCount = thrSolutions.Count,
                    AlgorithmResults = results,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new EQPGameResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        public static bool ValidateSolution(List<QueenPosition> positions, int boardSize)
        {
            return EQPSolver.ValidateSolution(positions, boardSize);
        }

        public static bool ValidateAndRecordPlayerSolution(Guid gameId, string playerName, List<QueenPosition> positions)
        {
            if (!rounds.ContainsKey(gameId)) throw new ArgumentException("Game not found");

            var round = rounds[gameId];
            var isValid = EQPSolver.ValidateSolution(positions, round.BoardSize);

            if (isValid)
            {
                // Build canonical signature to avoid duplicates
                var arr = new int[round.BoardSize];
                foreach (var p in positions)
                {
                    arr[p.Row] = p.Col;
                }
                var sig = Signature(arr);
                if (!recordedSolutions.Contains(sig))
                {
                    recordedSolutions.Add(sig);
                    savedPlayerSolutions.Add((gameId, playerName ?? "Anonymous", positions));
                    // TODO: persist to DB
                }
                // if already recorded, we still return true but inform frontend it was already recognized
            }

            return isValid;
        }

        private static string Signature(int[] sol)
        {
            return string.Join(",", sol);
        }

        private static string Signature(List<int[]> solList)
        {
            if (solList == null || solList.Count == 0) return "";
            return Signature(solList[0]);
        }

        private static string Signature(List<QueenPosition> positions)
        {
            var arr = positions.OrderBy(p => p.Row).Select(p => p.Col).ToArray();
            return string.Join(",", arr);
        }
    }
}
