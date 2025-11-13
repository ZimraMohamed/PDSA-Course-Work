using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PDSA.Core.Algorithms.TravelingSalesman
{
    public class TSPSolver
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// Generates random distance matrix with distances between 50-100km
        /// </summary>
        public static int[,] GenerateRandomDistanceMatrix()
        {
            int[,] distances = new int[10, 10];
            
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (i == j)
                    {
                        distances[i, j] = 0;
                    }
                    else if (i < j)
                    {
                        // Generate random distance between 50-100 km
                        distances[i, j] = random.Next(50, 101);
                        distances[j, i] = distances[i, j]; // Symmetric matrix
                    }
                }
            }
            
            return distances;
        }

        /// <summary>
        /// Selects a random home city
        /// </summary>
        public static City SelectRandomHomeCity()
        {
            int randomIndex = random.Next(0, 10);
            return TSPCities.AllCities[randomIndex];
        }

        /// <summary>
        /// Algorithm 1: Brute Force (Recursive) - O(n!)
        /// Finds optimal solution by checking all permutations
        /// </summary>
        public static TSPAlgorithmResult SolveBruteForce(int[,] distances, City homeCity, List<City> citiesToVisit)
        {
            var stopwatch = Stopwatch.StartNew();
            
            var allCities = new List<City>(citiesToVisit);
            if (!allCities.Contains(homeCity))
                allCities.Insert(0, homeCity);
            
            var otherCities = allCities.Where(c => c.Index != homeCity.Index).ToList();
            var bestRoute = new TSPRoute { TotalDistance = int.MaxValue };
            
            // Generate all permutations and find minimum
            GeneratePermutations(otherCities, 0, ref bestRoute, distances, homeCity.Index);
            
            stopwatch.Stop();
            
            return new TSPAlgorithmResult
            {
                Route = bestRoute,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                AlgorithmName = "Brute Force (Recursive)",
                Complexity = Factorial(otherCities.Count)
            };
        }

        /// <summary>
        /// Algorithm 2: Dynamic Programming with Bitmask - O(n²*2^n)
        /// Uses memoization to solve TSP efficiently
        /// </summary>
        public static TSPAlgorithmResult SolveDynamicProgramming(int[,] distances, City homeCity, List<City> citiesToVisit)
        {
            var stopwatch = Stopwatch.StartNew();
            
            var allCities = new List<City>(citiesToVisit);
            if (!allCities.Contains(homeCity))
                allCities.Insert(0, homeCity);
            
            int n = allCities.Count;
            int homeCityIndex = homeCity.Index;
            
            // Create city index mapping
            var cityIndexMap = new Dictionary<int, int>();
            for (int i = 0; i < n; i++)
            {
                cityIndexMap[allCities[i].Index] = i;
            }
            
            // DP table: dp[mask][i] = minimum cost to visit all cities in mask ending at city i
            var dp = new int[1 << n, n];
            var parent = new int[1 << n, n];
            
            // Initialize DP table
            for (int i = 0; i < (1 << n); i++)
            {
                for (int j = 0; j < n; j++)
                {
                    dp[i, j] = int.MaxValue;
                    parent[i, j] = -1;
                }
            }
            
            int homeIndex = cityIndexMap[homeCityIndex];
            dp[1 << homeIndex, homeIndex] = 0;
            
            // Fill DP table
            for (int mask = 0; mask < (1 << n); mask++)
            {
                for (int u = 0; u < n; u++)
                {
                    if ((mask & (1 << u)) == 0 || dp[mask, u] == int.MaxValue)
                        continue;
                    
                    for (int v = 0; v < n; v++)
                    {
                        if ((mask & (1 << v)) != 0) continue;
                        
                        int newMask = mask | (1 << v);
                        int newCost = dp[mask, u] + distances[allCities[u].Index, allCities[v].Index];
                        
                        if (newCost < dp[newMask, v])
                        {
                            dp[newMask, v] = newCost;
                            parent[newMask, v] = u;
                        }
                    }
                }
            }
            
            // Find minimum cost to return to home city
            int finalMask = (1 << n) - 1;
            int minCost = int.MaxValue;
            int lastCity = -1;
            
            for (int i = 0; i < n; i++)
            {
                if (i == homeIndex) continue;
                int cost = dp[finalMask, i] + distances[allCities[i].Index, homeCityIndex];
                if (cost < minCost)
                {
                    minCost = cost;
                    lastCity = i;
                }
            }
            
            // Reconstruct path
            var path = ReconstructPath(parent, finalMask, lastCity, homeIndex);
            var route = new TSPRoute
            {
                Cities = path.Select(i => allCities[i]).ToList(),
                TotalDistance = minCost,
                Path = path.Select(i => allCities[i].Index).ToList()
            };
            
            stopwatch.Stop();
            
            return new TSPAlgorithmResult
            {
                Route = route,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                AlgorithmName = "Dynamic Programming",
                Complexity = n * n * (1 << n)
            };
        }

        /// <summary>
        /// Algorithm 3: Nearest Neighbor Heuristic (Iterative) - O(n²)
        /// Greedy approach that may not find optimal solution but is fast
        /// </summary>
        public static TSPAlgorithmResult SolveNearestNeighbor(int[,] distances, City homeCity, List<City> citiesToVisit)
        {
            var stopwatch = Stopwatch.StartNew();
            
            var allCities = new List<City>(citiesToVisit);
            if (!allCities.Contains(homeCity))
                allCities.Insert(0, homeCity);
            
            var unvisited = new HashSet<int>(allCities.Select(c => c.Index));
            var path = new List<int>();
            var routeCities = new List<City>();
            
            int currentCity = homeCity.Index;
            int totalDistance = 0;
            
            path.Add(currentCity);
            routeCities.Add(homeCity);
            unvisited.Remove(currentCity);
            
            // Visit nearest unvisited city iteratively
            while (unvisited.Count > 0)
            {
                int nearestCity = -1;
                int minDistance = int.MaxValue;
                
                foreach (int city in unvisited)
                {
                    if (distances[currentCity, city] < minDistance)
                    {
                        minDistance = distances[currentCity, city];
                        nearestCity = city;
                    }
                }
                
                totalDistance += minDistance;
                currentCity = nearestCity;
                path.Add(currentCity);
                routeCities.Add(TSPCities.GetCityByIndex(currentCity));
                unvisited.Remove(currentCity);
            }
            
            // Return to home city
            totalDistance += distances[currentCity, homeCity.Index];
            
            var route = new TSPRoute
            {
                Cities = routeCities,
                TotalDistance = totalDistance,
                Path = path
            };
            
            stopwatch.Stop();
            
            return new TSPAlgorithmResult
            {
                Route = route,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                AlgorithmName = "Nearest Neighbor (Iterative)",
                Complexity = allCities.Count * allCities.Count
            };
        }

        #region Helper Methods

        private static void GeneratePermutations(List<City> cities, int start, ref TSPRoute bestRoute, int[,] distances, int homeCity)
        {
            if (start >= cities.Count)
            {
                int distance = CalculateRouteDistance(cities, distances, homeCity);
                if (distance < bestRoute.TotalDistance)
                {
                    bestRoute.TotalDistance = distance;
                    bestRoute.Cities = new List<City> { TSPCities.GetCityByIndex(homeCity) };
                    bestRoute.Cities.AddRange(cities);
                    bestRoute.Path = bestRoute.Cities.Select(c => c.Index).ToList();
                }
                return;
            }

            for (int i = start; i < cities.Count; i++)
            {
                Swap(cities, start, i);
                GeneratePermutations(cities, start + 1, ref bestRoute, distances, homeCity);
                Swap(cities, start, i); // backtrack
            }
        }

        private static int CalculateRouteDistance(List<City> cities, int[,] distances, int homeCity)
        {
            int total = 0;
            int current = homeCity;
            
            foreach (var city in cities)
            {
                total += distances[current, city.Index];
                current = city.Index;
            }
            
            total += distances[current, homeCity]; // Return to home
            return total;
        }

        private static void Swap(List<City> list, int i, int j)
        {
            (list[i], list[j]) = (list[j], list[i]);
        }

        private static List<int> ReconstructPath(int[,] parent, int mask, int lastCity, int homeCity)
        {
            var path = new List<int>();
            int currentMask = mask;
            int currentCity = lastCity;
            
            while (parent[currentMask, currentCity] != -1)
            {
                path.Add(currentCity);
                int nextCity = parent[currentMask, currentCity];
                currentMask ^= (1 << currentCity);
                currentCity = nextCity;
            }
            
            path.Add(homeCity);
            path.Reverse();
            return path;
        }

        private static int Factorial(int n)
        {
            if (n <= 1) return 1;
            return n * Factorial(n - 1);
        }

        #endregion
    }
}