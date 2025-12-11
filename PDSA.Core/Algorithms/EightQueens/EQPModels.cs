using System;
using System.Collections.Generic;

namespace PDSA.Core.Algorithms.EightQueens
{
    public class EQPGameRound
    {
        public Guid GameId { get; set; } = Guid.NewGuid();
        public int BoardSize { get; set; } = 8;
        // Optionally store solutions cache
    }

    public class QueenPosition
    {
        public int Row { get; set; }
        public int Col { get; set; }
    }

    public class EQPAlgorithmResult
    {
        public string AlgorithmName { get; set; } = "";
        public int SolutionsFound { get; set; }
        public long ExecutionTimeMs { get; set; }
    }

    public class EQPGameResult
    {
        public EQPGameRound Round { get; set; } = new EQPGameRound();
        public int SequentialSolutionsCount { get; set; }
        public int ThreadedSolutionsCount { get; set; }
        public List<EQPAlgorithmResult> AlgorithmResults { get; set; } = new();
        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }

    public class TOHPUserMoveRequest
{
    public int NumPegs { get; set; }
    public int NumDisks { get; set; }
    public string UserSequence { get; set; } = string.Empty;
}

}
