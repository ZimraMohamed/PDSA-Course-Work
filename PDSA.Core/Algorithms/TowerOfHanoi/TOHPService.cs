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

        // Check user sequence - validate if it actually solves the puzzle correctly
        var userSequenceList = request.UserSequence
                                      .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(m => m.Trim())
                                      .ToList();
        
        // Determine source and target based on number of pegs
        char source = 'A';
        char target = request.NumPegs == 3 ? 'C' : 'D';
        
        // Use validator to check if sequence is actually correct (not just matching our algorithm)
        bool correctSequence = TOHPSolver.ValidateSequence(userSequenceList, request.NumDisks, request.NumPegs, source, target);

        string message;
        if (correctMoves && correctSequence)
            message = "Move count and sequence are correct! ✅";
        else if (!correctMoves && correctSequence)
            message = $"Sequence is valid but not optimal. Optimal moves: {optimalMoves}, Your moves: {request.UserMovesCount}";
        else if (correctMoves && !correctSequence)
            message = $"Move count is correct but sequence is invalid (doesn't solve the puzzle correctly).";
        else
            message = $"Both move count and sequence are incorrect. Optimal moves: {optimalMoves}";

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
