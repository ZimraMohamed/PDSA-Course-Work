using PDSA.Core.Algorithms.TravelingSalesman;

namespace PDSA.Tests.Algorithms.TravelingSalesman
{
    public class TSPGameServiceTests
    {
        [Fact]
        public void CreateNewGameRound_Should_ReturnValidGameRound()
        {
            // Act
            var gameRound = TSPGameService.CreateNewGameRound();

            // Assert
            Assert.NotNull(gameRound);
            Assert.NotNull(gameRound.DistanceMatrix);
            Assert.NotNull(gameRound.HomeCity);

            // Validate distance matrix dimensions
            Assert.Equal(10, gameRound.DistanceMatrix.GetLength(0));
            Assert.Equal(10, gameRound.DistanceMatrix.GetLength(1));

            // Validate home city
            Assert.Contains(gameRound.HomeCity, TSPCities.AllCities);
        }

        [Fact]
        public void ValidateSelectedCities_Should_ReturnTrue_ForValidSelection()
        {
            // Arrange
            var selectedCityNames = new List<char> { 'A', 'B', 'C' };

            // Act
            var result = TSPGameService.ValidateSelectedCities(selectedCityNames);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateSelectedCities_Should_ReturnFalse_ForInsufficientCities()
        {
            // Arrange
            var selectedCityNames = new List<char> { 'A' };

            // Act
            var result = TSPGameService.ValidateSelectedCities(selectedCityNames);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateSelectedCities_Should_ReturnFalse_ForDuplicateCities()
        {
            // Arrange
            var selectedCityNames = new List<char> { 'A', 'B', 'A' };

            // Act
            var result = TSPGameService.ValidateSelectedCities(selectedCityNames);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateSelectedCities_Should_ReturnFalse_ForInvalidCityName()
        {
            // Arrange - Using invalid city names (outside A-J range)
            var selectedCityNames = new List<char> { 'A', 'Z', 'B' };

            // Act
            var result = TSPGameService.ValidateSelectedCities(selectedCityNames);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateSelectedCities_Should_ReturnFalse_ForEmptyList()
        {
            // Act & Assert
            Assert.False(TSPGameService.ValidateSelectedCities(new List<char>()));
        }

        [Fact]
        public void SolveTSPWithAllAlgorithms_Should_ReturnValidResults()
        {
            // Arrange
            var gameRound = TSPGameService.CreateNewGameRound();
            var selectedCityNames = new List<char> { 'A', 'B', 'C', 'D' };

            // Act
            var solution = TSPGameService.SolveTSPWithAllAlgorithms(gameRound, selectedCityNames);

            // Assert
            Assert.NotNull(solution);
            Assert.True(solution.Success);
            Assert.Null(solution.ErrorMessage);
            Assert.NotNull(solution.AlgorithmResults);
            Assert.Equal(3, solution.AlgorithmResults.Count);

            // Check that we have all three algorithms
            var algorithmNames = solution.AlgorithmResults.Select(r => r.AlgorithmName).ToList();
            Assert.Contains("Brute Force (Recursive)", algorithmNames);
            Assert.Contains("Dynamic Programming", algorithmNames);
            Assert.Contains("Nearest Neighbor (Iterative)", algorithmNames);

            // All results should have valid data
            foreach (var result in solution.AlgorithmResults)
            {
                Assert.NotNull(result.Route);
                Assert.True(result.Route.TotalDistance > 0);
                Assert.True(result.ExecutionTimeMs >= 0);
                Assert.NotEmpty(result.AlgorithmName);
            }

            // Optimal solution should be valid
            Assert.True(solution.OptimalDistance > 0);
            Assert.NotNull(solution.OptimalRoute);
        }

        [Fact]
        public void SolveTSPWithAllAlgorithms_Should_HandleInvalidCities()
        {
            // Arrange
            var gameRound = TSPGameService.CreateNewGameRound();
            var invalidCityNames = new List<char> { 'X', 'Y', 'Z' }; // Invalid cities

            // Act
            var solution = TSPGameService.SolveTSPWithAllAlgorithms(gameRound, invalidCityNames);

            // Assert
            Assert.NotNull(solution);
            Assert.False(solution.Success);
            Assert.NotNull(solution.ErrorMessage);
        }

        [Theory]
        [InlineData(100, 5, true)]  // Within 5% tolerance
        [InlineData(105, 5, true)]  // Exactly at 5% tolerance
        [InlineData(110, 5, false)] // Outside 5% tolerance
        [InlineData(95, 5, true)]   // Below optimal (should be accepted)
        [InlineData(100, 10, true)] // Within 10% tolerance
        [InlineData(111, 10, false)] // Outside 10% tolerance
        public void ValidateUserAnswer_Should_CalculateCorrectToleranceAcceptance(
            int userAnswer, int tolerancePercent, bool expectedResult)
        {
            // Arrange
            int correctAnswer = 100;

            // Act
            var result = TSPGameService.ValidateUserAnswer(userAnswer, correctAnswer, tolerancePercent);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void GetDistanceMatrixDisplay_Should_ReturnFormattedString()
        {
            // Arrange
            var distances = new int[10, 10];
            distances[0, 1] = distances[1, 0] = 50;
            distances[0, 2] = distances[2, 0] = 75;
            distances[1, 2] = distances[2, 1] = 60;
            
            var cities = new List<City> { TSPCities.AllCities[0], TSPCities.AllCities[1], TSPCities.AllCities[2] };

            // Act
            var display = TSPGameService.GetDistanceMatrixDisplay(distances, cities);

            // Assert
            Assert.NotNull(display);
            Assert.NotEmpty(display);
            Assert.Contains("A", display); // Should contain city names
            Assert.Contains("B", display);
            Assert.Contains("C", display);
            Assert.Contains("50", display); // Should contain distance values
        }

        [Fact]
        public void AnalyzeComplexity_Should_ReturnValidAnalysis()
        {
            // Arrange
            var results = new List<TSPAlgorithmResult>
            {
                new() { 
                    AlgorithmName = "Brute Force", 
                    ExecutionTimeMs = 100,
                    Route = new TSPRoute { TotalDistance = 150 }
                },
                new() { 
                    AlgorithmName = "Dynamic Programming", 
                    ExecutionTimeMs = 50,
                    Route = new TSPRoute { TotalDistance = 150 }
                },
                new() { 
                    AlgorithmName = "Nearest Neighbor", 
                    ExecutionTimeMs = 10,
                    Route = new TSPRoute { TotalDistance = 155 }
                }
            };

            // Act
            var analysis = TSPGameService.AnalyzeComplexity(results, 4);

            // Assert
            Assert.NotNull(analysis);
            Assert.NotNull(analysis.ExecutionTimes);
            Assert.True(analysis.ExecutionTimes.Count > 0);
            Assert.NotEmpty(analysis.BruteForceComplexity);
            Assert.NotEmpty(analysis.DynamicProgrammingComplexity);
            Assert.NotEmpty(analysis.NearestNeighborComplexity);
        }

        [Fact]
        public void TSPCities_AllCities_Should_HaveValidStructure()
        {
            // Assert
            Assert.NotNull(TSPCities.AllCities);
            Assert.Equal(10, TSPCities.AllCities.Length);
            
            // Check each city
            for (int i = 0; i < TSPCities.AllCities.Length; i++)
            {
                var city = TSPCities.AllCities[i];
                Assert.NotNull(city);
                Assert.Equal(i, city.Index);
                Assert.Equal((char)('A' + i), city.Name);
            }
        }

        [Fact]
        public void CreateNewGameRound_Should_GenerateDifferentMatricesOnMultipleCalls()
        {
            // Act
            var gameRound1 = TSPGameService.CreateNewGameRound();
            var gameRound2 = TSPGameService.CreateNewGameRound();

            // Assert - At least one value should be different (highly probable with random generation)
            bool hasDifference = false;
            for (int i = 0; i < 10 && !hasDifference; i++)
            {
                for (int j = 0; j < 10 && !hasDifference; j++)
                {
                    if (gameRound1.DistanceMatrix[i, j] != gameRound2.DistanceMatrix[i, j])
                    {
                        hasDifference = true;
                    }
                }
            }
            Assert.True(hasDifference);
        }

        [Fact]
        public void ValidateSelectedCities_Should_ReturnTrue_ForMaximumCities()
        {
            // Arrange - Select all 10 cities
            var selectedCityNames = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };

            // Act
            var result = TSPGameService.ValidateSelectedCities(selectedCityNames);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateSelectedCities_Should_ReturnFalse_ForNullList()
        {
            // Act & Assert
            Assert.False(TSPGameService.ValidateSelectedCities(null));
        }

        [Theory]
        [InlineData('K')]
        [InlineData('Z')]
        [InlineData('a')]
        [InlineData('1')]
        [InlineData('@')]
        public void ValidateSelectedCities_Should_ReturnFalse_ForVariousInvalidCharacters(char invalidChar)
        {
            // Arrange
            var selectedCityNames = new List<char> { 'A', 'B', invalidChar };

            // Act
            var result = TSPGameService.ValidateSelectedCities(selectedCityNames);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void SolveTSPWithAllAlgorithms_Should_ReturnOptimalSolution()
        {
            // Arrange
            var gameRound = TSPGameService.CreateNewGameRound();
            var selectedCityNames = new List<char> { 'A', 'B', 'C' };

            // Act
            var solution = TSPGameService.SolveTSPWithAllAlgorithms(gameRound, selectedCityNames);

            // Assert - Brute Force and DP should have the same optimal distance
            var bruteForceResult = solution.AlgorithmResults.First(r => r.AlgorithmName.Contains("Brute Force"));
            var dpResult = solution.AlgorithmResults.First(r => r.AlgorithmName.Contains("Dynamic Programming"));
            
            Assert.Equal(bruteForceResult.Route.TotalDistance, dpResult.Route.TotalDistance);
            Assert.Equal(solution.OptimalDistance, bruteForceResult.Route.TotalDistance);
        }

        [Fact]
        public void SolveTSPWithAllAlgorithms_Should_HaveNearestNeighborAsGreedyHeuristic()
        {
            // Arrange
            var gameRound = TSPGameService.CreateNewGameRound();
            var selectedCityNames = new List<char> { 'A', 'B', 'C', 'D' };

            // Act
            var solution = TSPGameService.SolveTSPWithAllAlgorithms(gameRound, selectedCityNames);

            // Assert - NN should not be better than optimal (it's a heuristic)
            var nnResult = solution.AlgorithmResults.First(r => r.AlgorithmName.Contains("Nearest Neighbor"));
            Assert.True(nnResult.Route.TotalDistance >= solution.OptimalDistance);
        }

        [Fact]
        public void SolveTSPWithAllAlgorithms_Should_RecordExecutionTimes()
        {
            // Arrange
            var gameRound = TSPGameService.CreateNewGameRound();
            var selectedCityNames = new List<char> { 'A', 'B', 'C', 'D', 'E' };

            // Act
            var solution = TSPGameService.SolveTSPWithAllAlgorithms(gameRound, selectedCityNames);

            // Assert
            foreach (var result in solution.AlgorithmResults)
            {
                Assert.True(result.ExecutionTimeMs >= 0, $"{result.AlgorithmName} should have non-negative execution time");
            }
        }

        [Theory]
        [InlineData(100, 100, 0, true)]  // Zero tolerance, exact match
        [InlineData(101, 100, 0, false)] // Zero tolerance, no match
        [InlineData(99, 100, 0, false)]  // Zero tolerance, no match (lower)
        [InlineData(50, 100, 5, false)]  // Far outside tolerance
        [InlineData(150, 100, 5, false)] // Far outside tolerance (higher)
        [InlineData(200, 200, 0, true)]  // Zero tolerance, different values, exact match
        public void ValidateUserAnswer_Should_HandleEdgeCases(
            int userAnswer, int correctAnswer, int tolerancePercent, bool expectedResult)
        {
            // Act
            var result = TSPGameService.ValidateUserAnswer(userAnswer, correctAnswer, tolerancePercent);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void GetDistanceMatrixDisplay_Should_HandleSingleCity()
        {
            // Arrange
            var distances = new int[10, 10];
            var cities = new List<City> { TSPCities.AllCities[0] };

            // Act
            var display = TSPGameService.GetDistanceMatrixDisplay(distances, cities);

            // Assert
            Assert.NotNull(display);
            Assert.Contains("A", display);
            Assert.Contains("--", display); // Self-distance should be marked
        }

        [Fact]
        public void GetDistanceMatrixDisplay_Should_ContainAllSelectedCities()
        {
            // Arrange
            var distances = TSPSolver.GenerateRandomDistanceMatrix();
            var cities = new List<City> { 
                TSPCities.AllCities[0], 
                TSPCities.AllCities[3], 
                TSPCities.AllCities[7] 
            };

            // Act
            var display = TSPGameService.GetDistanceMatrixDisplay(distances, cities);

            // Assert
            Assert.Contains("A", display);
            Assert.Contains("D", display);
            Assert.Contains("H", display);
        }

        [Fact]
        public void AnalyzeComplexity_Should_CalculateFactorialCorrectly()
        {
            // Arrange
            var results = new List<TSPAlgorithmResult>
            {
                new() { 
                    AlgorithmName = "Brute Force", 
                    ExecutionTimeMs = 10,
                    Route = new TSPRoute { TotalDistance = 100 }
                },
                new() { 
                    AlgorithmName = "Dynamic Programming", 
                    ExecutionTimeMs = 5,
                    Route = new TSPRoute { TotalDistance = 100 }
                },
                new() { 
                    AlgorithmName = "Nearest Neighbor", 
                    ExecutionTimeMs = 1,
                    Route = new TSPRoute { TotalDistance = 105 }
                }
            };

            // Act
            var analysis = TSPGameService.AnalyzeComplexity(results, 5);

            // Assert
            Assert.Contains("O(120)", analysis.BruteForceComplexity); // 5! = 120
            Assert.NotEmpty(analysis.DynamicProgrammingComplexity);
            Assert.NotEmpty(analysis.NearestNeighborComplexity);
        }

        [Fact]
        public void AnalyzeComplexity_Should_IncludeAllAlgorithmTimes()
        {
            // Arrange
            var results = new List<TSPAlgorithmResult>
            {
                new() { 
                    AlgorithmName = "Brute Force", 
                    ExecutionTimeMs = 50,
                    Route = new TSPRoute { TotalDistance = 100 }
                },
                new() { 
                    AlgorithmName = "Dynamic Programming", 
                    ExecutionTimeMs = 30,
                    Route = new TSPRoute { TotalDistance = 100 }
                },
                new() { 
                    AlgorithmName = "Nearest Neighbor", 
                    ExecutionTimeMs = 5,
                    Route = new TSPRoute { TotalDistance = 105 }
                }
            };

            // Act
            var analysis = TSPGameService.AnalyzeComplexity(results, 6);

            // Assert
            Assert.Equal(3, analysis.ExecutionTimes.Count);
            Assert.Equal(50, analysis.ExecutionTimes["Brute Force"]);
            Assert.Equal(30, analysis.ExecutionTimes["Dynamic Programming"]);
            Assert.Equal(5, analysis.ExecutionTimes["Nearest Neighbor"]);
        }

        [Fact]
        public void AnalyzeComplexity_Should_IndicateOptimalSolutionFound()
        {
            // Arrange
            var results = new List<TSPAlgorithmResult>
            {
                new() { 
                    AlgorithmName = "Brute Force", 
                    ExecutionTimeMs = 10,
                    Route = new TSPRoute { TotalDistance = 100 }
                },
                new() { 
                    AlgorithmName = "Dynamic Programming", 
                    ExecutionTimeMs = 8,
                    Route = new TSPRoute { TotalDistance = 100 }
                }
            };

            // Act
            var analysis = TSPGameService.AnalyzeComplexity(results, 3);

            // Assert
            Assert.True(analysis.OptimalSolutionFound);
        }

        [Fact]
        public void SolveTSPWithAllAlgorithms_Should_HandleMinimumCitySelection()
        {
            // Arrange
            var gameRound = TSPGameService.CreateNewGameRound();
            var selectedCityNames = new List<char> { 'A', 'B' }; // Minimum 2 cities

            // Act
            var solution = TSPGameService.SolveTSPWithAllAlgorithms(gameRound, selectedCityNames);

            // Assert
            Assert.True(solution.Success);
            Assert.NotNull(solution.AlgorithmResults);
            Assert.Equal(3, solution.AlgorithmResults.Count);
        }

        [Fact]
        public void SolveTSPWithAllAlgorithms_Should_AllReturnValidRoutes()
        {
            // Arrange
            var gameRound = TSPGameService.CreateNewGameRound();
            var selectedCityNames = new List<char> { 'A', 'B', 'C', 'D' };

            // Act
            var solution = TSPGameService.SolveTSPWithAllAlgorithms(gameRound, selectedCityNames);

            // Assert
            foreach (var result in solution.AlgorithmResults)
            {
                Assert.NotNull(result.Route);
                Assert.NotNull(result.Route.Cities);
                Assert.True(result.Route.Cities.Count >= 2);
                Assert.NotNull(result.Route.Path);
                Assert.True(result.Route.Path.Count >= 2);
            }
        }

        [Fact]
        public void ValidateUserAnswer_Should_AcceptAnswersWithinTolerance()
        {
            // Arrange - 100km optimal, 10% tolerance = 100-110km range
            int correctAnswer = 100;
            
            // Act & Assert
            Assert.True(TSPGameService.ValidateUserAnswer(100, correctAnswer, 10));
            Assert.True(TSPGameService.ValidateUserAnswer(105, correctAnswer, 10));
            Assert.True(TSPGameService.ValidateUserAnswer(110, correctAnswer, 10));
            Assert.True(TSPGameService.ValidateUserAnswer(95, correctAnswer, 10));  // Below is also acceptable
            Assert.True(TSPGameService.ValidateUserAnswer(90, correctAnswer, 10));
        }

        [Fact]
        public void ValidateUserAnswer_Should_RejectAnswersOutsideTolerance()
        {
            // Arrange - 100km optimal, 10% tolerance
            int correctAnswer = 100;
            
            // Act & Assert
            Assert.False(TSPGameService.ValidateUserAnswer(111, correctAnswer, 10));
            Assert.False(TSPGameService.ValidateUserAnswer(89, correctAnswer, 10));
            Assert.False(TSPGameService.ValidateUserAnswer(120, correctAnswer, 10));
            Assert.False(TSPGameService.ValidateUserAnswer(80, correctAnswer, 10));
        }
    }
}