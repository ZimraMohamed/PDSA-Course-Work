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
        public bool CorrectMoves { get; set; }           // is move count correct
        public bool CorrectSequence { get; set; }        // is sequence correct
        public List<string> CorrectSequenceList { get; set; } = new List<string>(); // correct move sequence
        public string Message { get; set; } = string.Empty;
    }
}
