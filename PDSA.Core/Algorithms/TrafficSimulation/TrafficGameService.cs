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

            var stopwatch = Stopwatch.StartNew();
            int maxFlow = solver.ComputeMaxFlow("A", "T");
            stopwatch.Stop();

            return new TrafficGameResult
            {
                PlayerAnswer = playerAnswer,
                CorrectAnswer = maxFlow,
                Status = playerAnswer == maxFlow ? "Correct" : "Wrong",
                TimeTaken = stopwatch.ElapsedMilliseconds
            };
        }
    }
}
