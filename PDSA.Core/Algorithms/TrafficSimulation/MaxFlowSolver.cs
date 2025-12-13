using System.Collections.Generic;
using System.Linq;

namespace PDSA.Core.Algorithms.TrafficSimulation
{
    public class MaxFlowSolver
    {
        private Dictionary<string, Dictionary<string, int>> capacity = new();

        public void AddEdge(string from, string to, int cap)
        {
            if (!capacity.ContainsKey(from))
                capacity[from] = new Dictionary<string, int>();

            capacity[from][to] = cap;
        }

        /// <summary>
        /// Algorithm 1: Edmonds-Karp Algorithm (BFS-based Ford-Fulkerson)
        /// Time Complexity: O(V * E^2)
        /// </summary>
        public int EdmondsKarp(string source, string sink)
        {
            // Create a copy of capacity for this algorithm
            var residualCapacity = DeepCopyCapacity();
            var flow = 0;

            while (true)
            {
                var parent = new Dictionary<string, string>();
                var queue = new Queue<string>();
                queue.Enqueue(source);

                while (queue.Count > 0)
                {
                    var node = queue.Dequeue();

                    if (!residualCapacity.ContainsKey(node)) continue;

                    foreach (var next in residualCapacity[node])
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
                    pathFlow = Math.Min(pathFlow, residualCapacity[u][v]);
                    v = u;
                }

                v = sink;
                while (v != source)
                {
                    string u = parent[v];
                    residualCapacity[u][v] -= pathFlow;

                    if (!residualCapacity.ContainsKey(v))
                        residualCapacity[v] = new Dictionary<string, int>();

                    if (!residualCapacity[v].ContainsKey(u))
                        residualCapacity[v][u] = 0;

                    residualCapacity[v][u] += pathFlow;

                    v = u;
                }

                flow += pathFlow;
            }

            return flow;
        }

        /// <summary>
        /// Algorithm 2: Dinic's Algorithm (Level Graph with Blocking Flow)
        /// Time Complexity: O(V^2 * E)
        /// Generally faster than Edmonds-Karp for dense graphs
        /// </summary>
        public int Dinic(string source, string sink)
        {
            // Create a copy of capacity for this algorithm
            var residualCapacity = DeepCopyCapacity();
            int maxFlow = 0;

            while (BFS_BuildLevelGraph(residualCapacity, source, sink, out var level))
            {
                var iter = new Dictionary<string, int>();
                int flow;
                while ((flow = DFS_SendFlow(residualCapacity, level, iter, source, sink, int.MaxValue)) > 0)
                {
                    maxFlow += flow;
                }
            }

            return maxFlow;
        }

        /// <summary>
        /// Build level graph using BFS for Dinic's algorithm
        /// </summary>
        private bool BFS_BuildLevelGraph(
            Dictionary<string, Dictionary<string, int>> residualCapacity,
            string source,
            string sink,
            out Dictionary<string, int> level)
        {
            level = new Dictionary<string, int>();
            level[source] = 0;

            var queue = new Queue<string>();
            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                var u = queue.Dequeue();

                if (!residualCapacity.ContainsKey(u)) continue;

                foreach (var kvp in residualCapacity[u])
                {
                    var v = kvp.Key;
                    var cap = kvp.Value;

                    if (!level.ContainsKey(v) && cap > 0)
                    {
                        level[v] = level[u] + 1;
                        queue.Enqueue(v);
                    }
                }
            }

            return level.ContainsKey(sink);
        }

        /// <summary>
        /// Send blocking flow using DFS for Dinic's algorithm
        /// </summary>
        private int DFS_SendFlow(
            Dictionary<string, Dictionary<string, int>> residualCapacity,
            Dictionary<string, int> level,
            Dictionary<string, int> iter,
            string u,
            string sink,
            int flow)
        {
            if (u == sink)
                return flow;

            if (!residualCapacity.ContainsKey(u))
                return 0;

            if (!iter.ContainsKey(u))
                iter[u] = 0;

            var edges = residualCapacity[u].ToList();

            for (int i = iter[u]; i < edges.Count; i++)
            {
                iter[u] = i;
                var v = edges[i].Key;
                var cap = edges[i].Value;

                if (level.ContainsKey(v) && level[v] == level[u] + 1 && cap > 0)
                {
                    int d = DFS_SendFlow(residualCapacity, level, iter, v, sink, Math.Min(flow, cap));

                    if (d > 0)
                    {
                        residualCapacity[u][v] -= d;

                        if (!residualCapacity.ContainsKey(v))
                            residualCapacity[v] = new Dictionary<string, int>();

                        if (!residualCapacity[v].ContainsKey(u))
                            residualCapacity[v][u] = 0;

                        residualCapacity[v][u] += d;

                        return d;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Legacy method - uses Edmonds-Karp for backward compatibility
        /// </summary>
        public int ComputeMaxFlow(string source, string sink)
        {
            return EdmondsKarp(source, sink);
        }

        /// <summary>
        /// Deep copy the capacity dictionary to preserve original graph
        /// </summary>
        private Dictionary<string, Dictionary<string, int>> DeepCopyCapacity()
        {
            var copy = new Dictionary<string, Dictionary<string, int>>();
            foreach (var kvp in capacity)
            {
                copy[kvp.Key] = new Dictionary<string, int>(kvp.Value);
            }
            return copy;
        }
    }
}
