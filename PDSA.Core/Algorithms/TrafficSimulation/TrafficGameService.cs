using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PDSA.Core.Algorithms.TrafficSimulation
{
    public class TrafficGameService
    {
        public TrafficGameResult CalculateMaxFlow(TrafficNetwork network, int playerAnswer)
        {
            var solver = new MaxFlowSolver();

            foreach (var e in network.Edges)
                solver.AddEdge(e.From, e.To, e.Capacity);

            // Run Edmonds-Karp algorithm
            var stopwatch1 = Stopwatch.StartNew();
            int maxFlowEdmondsKarp = solver.EdmondsKarp("A", "T");
            stopwatch1.Stop();

            // Run Dinic's algorithm
            var stopwatch2 = Stopwatch.StartNew();
            int maxFlowDinic = solver.Dinic("A", "T");
            stopwatch2.Stop();

            return new TrafficGameResult
            {
                PlayerAnswer = playerAnswer,
                CorrectAnswer = maxFlowEdmondsKarp, // Both should give same result
                Status = playerAnswer == maxFlowEdmondsKarp ? "Correct" : "Wrong",
                EdmondsKarpTime = stopwatch1.ElapsedMilliseconds,
                DinicTime = stopwatch2.ElapsedMilliseconds,
                TimeTaken = stopwatch1.ElapsedMilliseconds // Keep for backward compatibility
            };
        }
    }
}
