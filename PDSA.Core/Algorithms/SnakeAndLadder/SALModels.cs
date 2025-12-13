namespace PDSA.Core.Algorithms.SnakeAndLadder
{
    public class Snake
    {
        public int Head { get; set; }
        public int Tail { get; set; }
    }

    public class Ladder
    {
        public int Bottom { get; set; }
        public int Top { get; set; }
    }

    public class SALRequest
    {
        public int BoardSize { get; set; }
        public List<Snake> Snakes { get; set; } = new List<Snake>();
        public List<Ladder> Ladders { get; set; } = new List<Ladder>();
    }

    public class SALAlgorithmResult
    {
        public string AlgorithmName { get; set; } = string.Empty;
        public int MinimumThrows { get; set; }
        public double ExecutionTimeMs { get; set; }
        public List<int> Path { get; set; } = new List<int>();
    }

    public class SALSolution
    {
        public string GameId { get; set; } = string.Empty;
        public int MinimumThrows { get; set; }
        public List<SALAlgorithmResult> AlgorithmResults { get; set; } = new List<SALAlgorithmResult>();
    }
}
