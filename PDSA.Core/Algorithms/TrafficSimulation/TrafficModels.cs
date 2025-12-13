namespace PDSA.Core.Algorithms.TrafficSimulation
{
    public class TrafficEdge
    {
        public string From { get; set; }
        public string To   { get; set; }
        public int Capacity { get; set; }
    }

    public class TrafficNetwork
    {
        public List<TrafficEdge> Edges { get; set; } = new();
    }

    public class TrafficGameResult
    {
        public int PlayerAnswer { get; set; }
        public int CorrectAnswer { get; set; }
        public long TimeTaken { get; set; } // Legacy - Edmonds-Karp time
        public long EdmondsKarpTime { get; set; } // Edmonds-Karp algorithm execution time
        public long DinicTime { get; set; } // Dinic's algorithm execution time
        public string Status { get; set; }
    }
}
