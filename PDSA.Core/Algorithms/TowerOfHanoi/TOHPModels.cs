using System.Collections.Generic;

namespace PDSA.API.Models
{
    // Request DTO: data sent from frontend
    public class TOHPRequest
    {
        public int NumPegs { get; set; }
        public int NumDisks { get; set; }
        public int UserMovesCount { get; set; }
        public string UserSequence { get; set; } = string.Empty;
    }

    // Response DTO: data returned to frontend
    public class TOHPResponse
    {
    public int OptimalMoves { get; set; }
    public int UserMoves { get; set; }
    public bool CorrectMoves { get; set; }           
    public bool CorrectSequence { get; set; }        
    public List<string> CorrectSequenceList { get; set; } = new List<string>();
    public string Message { get; set; } = string.Empty;

    // New properties
    public string AlgorithmName { get; set; } = "";
    public long AlgorithmTimeMs { get; set; } = 0;

    // For multiple algorithms
    public Dictionary<string, long> BenchmarkTimings { get; set; } = new();
    }
}