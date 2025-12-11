using PDSA.API.Models;
using PDSA.Core.Algorithms.TowerOfHanoi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDSA.API.Services
{
    public class TOHPService
    {
        public TOHPResponse CheckUserMoves(TOHPRequest request)
        {
            List<string> optimalSequence;

            // Generate optimal sequence based on peg count
            if (request.NumPegs == 3)
            {
                optimalSequence = TOHPSolver.SolveRecursive(request.NumDisks, 'A', 'C', 'B');
            }
            else if (request.NumPegs == 4)
            {
                optimalSequence = TOHPSolver.Solve4Pegs_FrameStewart(request.NumDisks, 'A', 'D', 'B', 'C');
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

            // Construct message
            string message;
            if (correctMoves && correctSequence)
                message = "Move count and sequence are correct! ✅";
            else if (!correctMoves)
                message = $"Incorrect number of moves. Optimal moves: {optimalMoves}";
            else
                message = $"Move count correct, but sequence is wrong. Correct sequence: {string.Join(", ", optimalSequence)}";

            return new TOHPResponse
            {
                OptimalMoves = optimalMoves,
                UserMoves = request.UserMovesCount,
                CorrectMoves = correctMoves,
                CorrectSequence = correctSequence,
                CorrectSequenceList = optimalSequence,
                Message = message
            };
        }
    }
}
