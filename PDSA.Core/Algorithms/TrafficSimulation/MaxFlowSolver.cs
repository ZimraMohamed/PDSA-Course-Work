using System.Collections.Generic;
using System.Linq;

namespace PDSA.Core.Algorithms.TrafficSimulation
{
    public class MaxFlowSolver
    {
        private readonly Dictionary<string, Dictionary<string, int>> capacity =
            new();

        public void AddEdge(string from, string to, int cap)
        {
            if (!capacity.ContainsKey(from))
                capacity[from] = new Dictionary<string, int>();

            capacity[from][to] = cap;
        }

        public int ComputeMaxFlow(string source, string sink)
        {
            var flow = 0;

            while (true)
            {
                var parent = new Dictionary<string, string>();
                var queue = new Queue<string>();
                queue.Enqueue(source);

                while (queue.Count > 0)
                {
                    var node = queue.Dequeue();

                    if (!capacity.ContainsKey(node)) continue;

                    foreach (var next in capacity[node])
                    {
                        if (next.Value > 0 && !parent.ContainsKey(next.Key))
                        {
                            parent[next.Key] = node;
                            queue.Enqueue(next.Key);

                            if (next.Key == sink)
                                break;
                        }
                    }
                }

                if (!parent.ContainsKey(sink))
                    break;

                int pathFlow = int.MaxValue;
                string v = sink;

                while (v != source)
                {
                    string u = parent[v];
                    pathFlow = Math.Min(pathFlow, capacity[u][v]);
                    v = u;
                }

                v = sink;
                while (v != source)
                {
                    string u = parent[v];
                    capacity[u][v] -= pathFlow;

                    if (!capacity.ContainsKey(v))
                        capacity[v] = new Dictionary<string, int>();

                    if (!capacity[v].ContainsKey(u))
                        capacity[v][u] = 0;

                    capacity[v][u] += pathFlow;

                    v = u;
                }

                flow += pathFlow;
            }

            return flow;
        }
    }
}
