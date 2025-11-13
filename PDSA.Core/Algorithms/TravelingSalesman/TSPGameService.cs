using System;
using System.Collections.Generic;
using System.Linq;

namespace PDSA.Core.Algorithms.TravelingSalesman
{
    public class TSPGameService
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// Creates a new TSP game round with random parameters
        /// </summary>
        public static TSPGameRound CreateNewGameRound()
        {
            var gameRound = new TSPGameRound
            {
                DistanceMatrix = TSPSolver.GenerateRandomDistanceMatrix(),
                HomeCity = TSPSolver.SelectRandomHomeCity()
            };

            return gameRound;
        }

        /// <summary>
        /// Validates if the selected cities are valid
        /// </summary>
        public static bool ValidateSelectedCities(List<char> selectedCityNames)
        {
            if (selectedCityNames == null || selectedCityNames.Count < 2)
                return false;

            // Check if all selected cities are valid (A-J)
            foreach (char cityName in selectedCityNames)
            {
                if (cityName < 'A' || cityName > 'J')
                    return false;
            }

            // Check for duplicates
            return selectedCityNames.Distinct().Count() == selectedCityNames.Count;
        }

        /// <summary>
        /// Solves TSP using all three algorithms and compares results
        /// </summary>
        public static TSPGameResult SolveTSPWithAllAlgorithms(TSPGameRound gameRound, List<char> selectedCityNames)
        {
            // Convert city names to City objects
            var selectedCities = selectedCityNames.Select(name => TSPCities.GetCityByName(name)).ToList();
            gameRound.SelectedCities = selectedCities;

            var results = new List<TSPAlgorithmResult>();

            try
            {
                // Algorithm 1: Brute Force (Recursive)
                var bruteForceResult = TSPSolver.SolveBruteForce(gameRound.DistanceMatrix, gameRound.HomeCity, selectedCities);
                results.Add(bruteForceResult);
                gameRound.BruteForceTime = bruteForceResult.ExecutionTimeMs;

                // Algorithm 2: Dynamic Programming
                var dpResult = TSPSolver.SolveDynamicProgramming(gameRound.DistanceMatrix, gameRound.HomeCity, selectedCities);
                results.Add(dpResult);
                gameRound.DynamicProgrammingTime = dpResult.ExecutionTimeMs;

                // Algorithm 3: Nearest Neighbor (Iterative)
                var nnResult = TSPSolver.SolveNearestNeighbor(gameRound.DistanceMatrix, gameRound.HomeCity, selectedCities);
                results.Add(nnResult);
                gameRound.NearestNeighborTime = nnResult.ExecutionTimeMs;

                // The optimal route is from brute force (guaranteed optimal) or DP (also optimal)
                gameRound.OptimalRoute = bruteForceResult.Route.TotalDistance <= dpResult.Route.TotalDistance 
                    ? bruteForceResult.Route 
                    : dpResult.Route;

                return new TSPGameResult
                {
                    GameRound = gameRound,
                    AlgorithmResults = results,
                    OptimalDistance = gameRound.OptimalRoute.TotalDistance,
                    OptimalRoute = gameRound.OptimalRoute,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new TSPGameResult
                {
                    GameRound = gameRound,
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Validates if the user's answer is correct (within tolerance)
        /// </summary>
        public static bool ValidateUserAnswer(int userAnswer, int correctAnswer, int tolerancePercentage = 5)
        {
            int tolerance = (int)(correctAnswer * tolerancePercentage / 100.0);
            return Math.Abs(userAnswer - correctAnswer) <= tolerance;
        }

        /// <summary>
        /// Generates a formatted string of the distance matrix for display
        /// </summary>
        public static string GetDistanceMatrixDisplay(int[,] distances, List<City> cities)
        {
            var result = new List<string>();
            result.Add("Distance Matrix (km):");
            result.Add("");

            // Header
            var header = "     ";
            foreach (var city in cities)
            {
                header += $"{city.Name,6}";
            }
            result.Add(header);

            // Rows
            foreach (var fromCity in cities)
            {
                var row = $"{fromCity.Name,3}: ";
                foreach (var toCity in cities)
                {
                    if (fromCity.Index == toCity.Index)
                        row += "   -- ";
                    else
                        row += $"{distances[fromCity.Index, toCity.Index],6}";
                }
                result.Add(row);
            }

            return string.Join("\n", result);
        }

        /// <summary>
        /// Analyzes algorithm complexities for reporting
        /// </summary>
        public static AlgorithmComplexityAnalysis AnalyzeComplexity(List<TSPAlgorithmResult> results, int numberOfCities)
        {
            return new AlgorithmComplexityAnalysis
            {
                BruteForceComplexity = $"O({numberOfCities}!) = O({Factorial(numberOfCities)})",
                DynamicProgrammingComplexity = $"O(n²×2^n) = O({numberOfCities}²×2^{numberOfCities}) = O({numberOfCities * numberOfCities * (1 << numberOfCities)})",
                NearestNeighborComplexity = $"O(n²) = O({numberOfCities}²) = O({numberOfCities * numberOfCities})",
                ExecutionTimes = results.ToDictionary(r => r.AlgorithmName, r => r.ExecutionTimeMs),
                OptimalSolutionFound = results.Where(r => r.AlgorithmName.Contains("Brute Force") || r.AlgorithmName.Contains("Dynamic")).Any()
            };
        }

        private static long Factorial(int n)
        {
            if (n <= 1) return 1;
            long result = 1;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }
    }

    public class TSPGameResult
    {
        public TSPGameRound? GameRound { get; set; }
        public List<TSPAlgorithmResult> AlgorithmResults { get; set; } = new List<TSPAlgorithmResult>();
        public int OptimalDistance { get; set; }
        public TSPRoute? OptimalRoute { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class AlgorithmComplexityAnalysis
    {
        public string BruteForceComplexity { get; set; } = "";
        public string DynamicProgrammingComplexity { get; set; } = "";
        public string NearestNeighborComplexity { get; set; } = "";
        public Dictionary<string, long> ExecutionTimes { get; set; } = new Dictionary<string, long>();
        public bool OptimalSolutionFound { get; set; }
    }
}