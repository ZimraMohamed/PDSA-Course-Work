using PDSA.API.Models;
using PDSA.Core.Algorithms.TowerOfHanoi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace PDSA.API.Services
{
    public class TOHPService
{
    public TOHPResponse CheckUserMoves(TOHPRequest request)
    {
        List<string> optimalSequence = null;
        var benchmark = new Dictionary<string, long>();

        if (request.NumPegs == 3)
        {
            // Measure Recursive
            var sw = Stopwatch.StartNew();
            var seqRecursive = TOHPSolver.SolveRecursive(request.NumDisks, 'A', 'C', 'B');
            sw.Stop();
            benchmark["3-Peg Recursive"] = sw.ElapsedMilliseconds;

            // Measure Iterative
            sw.Restart();
            var seqIterative = TOHPSolver.SolveIterative(request.NumDisks);
            sw.Stop();
            benchmark["3-Peg Iterative"] = sw.ElapsedMilliseconds;

            // Pick one as optimal (say Recursive)
            optimalSequence = seqRecursive;
        }
        else if (request.NumPegs == 4)
        {
            // Measure Frame-Stewart
            var sw = Stopwatch.StartNew();
            var seqFS = TOHPSolver.Solve4Pegs_FrameStewart(request.NumDisks, 'A', 'D', 'B', 'C');
            sw.Stop();
            benchmark["4-Peg Frame-Stewart"] = sw.ElapsedMilliseconds;

            // Measure Balanced
            sw.Restart();
            var seqBalanced = TOHPSolver.Solve4Pegs_Balanced(request.NumDisks, 'A', 'D', 'B', 'C');
            sw.Stop();
            benchmark["4-Peg Balanced"] = sw.ElapsedMilliseconds;

            // Pick one as optimal (say Frame-Stewart)
            optimalSequence = seqFS;
        }
        else
        {
            throw new Exception("Unsupported peg count. Only 3 or 4 pegs allowed.");
        }

        int optimalMoves = optimalSequence.Count;

        // Check user move count
        bool correctMoves = request.UserMovesCount == optimalMoves;

        // Check user sequence
        var userSequenceList = request.UserSequence
                                      .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(m => m.Trim())
                                      .ToList();
        bool correctSequence = userSequenceList.SequenceEqual(optimalSequence);

        string message;
        if (correctMoves && correctSequence)
            message = "Move count and sequence are correct! ✅";
        else if (!correctMoves)
            message = $"Incorrect number of moves. Optimal moves: {optimalMoves}";
        else
            message = $"Move count correct, but sequence is wrong.";

        return new TOHPResponse
        {
            OptimalMoves = optimalMoves,
            UserMoves = request.UserMovesCount,
            CorrectMoves = correctMoves,
            CorrectSequence = correctSequence,
            CorrectSequenceList = optimalSequence,
            Message = message,
            AlgorithmName = benchmark.Keys.First(),       // The "main" algorithm
            AlgorithmTimeMs = benchmark.Values.First(),   // Its time
            BenchmarkTimings = benchmark                 // All algorithm timings
        };
    }
}
}
