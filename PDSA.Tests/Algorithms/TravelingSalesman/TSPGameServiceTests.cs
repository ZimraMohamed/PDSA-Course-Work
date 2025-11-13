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
            Assert.Contains("Brute Force", algorithmNames);
            Assert.Contains("Dynamic Programming", algorithmNames);
            Assert.Contains("Nearest Neighbor", algorithmNames);

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
    }
}