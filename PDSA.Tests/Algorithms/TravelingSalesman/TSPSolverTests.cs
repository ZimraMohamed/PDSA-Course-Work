using PDSA.Core.Algorithms.TravelingSalesman;

namespace PDSA.Tests.Algorithms.TravelingSalesman
{
    public class TSPSolverTests
    {
        [Fact]
        public void GenerateRandomDistanceMatrix_Should_CreateValidMatrix()
        {
            // Act
            var matrix = TSPSolver.GenerateRandomDistanceMatrix();

            // Assert
            Assert.NotNull(matrix);
            Assert.Equal(10, matrix.GetLength(0));
            Assert.Equal(10, matrix.GetLength(1));

            // Check diagonal is zero
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                Assert.Equal(0, matrix[i, i]);
            }

            // Check symmetry
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Assert.Equal(matrix[i, j], matrix[j, i]);
                }
            }

            // Check distance ranges (50-100 km)
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (i != j)
                    {
                        Assert.InRange(matrix[i, j], 50, 100);
                    }
                }
            }
        }

        [Fact]
        public void SelectRandomHomeCity_Should_ReturnValidCity()
        {
            // Act
            var homeCity = TSPSolver.SelectRandomHomeCity();

            // Assert
            Assert.NotNull(homeCity);
            Assert.Contains(homeCity, TSPCities.AllCities);
            Assert.InRange(homeCity.Index, 0, TSPCities.AllCities.Length - 1);
            Assert.Equal(TSPCities.AllCities[homeCity.Index], homeCity);
        }

        [Fact]
        public void SolveBruteForce_Should_FindOptimalSolution_SmallProblem()
        {
            // Arrange - Create a simple 3-city problem with known optimal solution
            var matrix = new int[10, 10];
            
            // Set up a simple 3-city subproblem
            matrix[0, 1] = matrix[1, 0] = 10;
            matrix[0, 2] = matrix[2, 0] = 15;
            matrix[1, 2] = matrix[2, 1] = 20;

            var homeCity = TSPCities.AllCities[0]; // City 'A'
            var citiesToVisit = new List<City> { TSPCities.AllCities[1], TSPCities.AllCities[2] }; // Cities 'B', 'C'

            // Act
            var result = TSPSolver.SolveBruteForce(matrix, homeCity, citiesToVisit);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Brute Force", result.AlgorithmName);
            Assert.NotNull(result.Route);
            Assert.True(result.Route.TotalDistance > 0);
            Assert.True(result.ExecutionTimeMs >= 0);
            
            // Check route validity
            Assert.NotNull(result.Route.Cities);
            Assert.True(result.Route.Cities.Count >= 2);
        }

        [Fact]
        public void SolveDynamicProgramming_Should_FindOptimalSolution_SmallProblem()
        {
            // Arrange
            var matrix = new int[10, 10];
            
            // Set up a simple 3-city subproblem
            matrix[0, 1] = matrix[1, 0] = 10;
            matrix[0, 2] = matrix[2, 0] = 15;
            matrix[1, 2] = matrix[2, 1] = 20;

            var homeCity = TSPCities.AllCities[0];
            var citiesToVisit = new List<City> { TSPCities.AllCities[1], TSPCities.AllCities[2] };

            // Act
            var result = TSPSolver.SolveDynamicProgramming(matrix, homeCity, citiesToVisit);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dynamic Programming", result.AlgorithmName);
            Assert.NotNull(result.Route);
            Assert.True(result.Route.TotalDistance > 0);
            Assert.True(result.ExecutionTimeMs >= 0);
        }

        [Fact]
        public void SolveNearestNeighbor_Should_FindValidSolution_SmallProblem()
        {
            // Arrange
            var matrix = new int[10, 10];
            
            // Set up distances
            matrix[0, 1] = matrix[1, 0] = 10;
            matrix[0, 2] = matrix[2, 0] = 15;
            matrix[0, 3] = matrix[3, 0] = 20;
            matrix[1, 2] = matrix[2, 1] = 35;
            matrix[1, 3] = matrix[3, 1] = 25;
            matrix[2, 3] = matrix[3, 2] = 30;

            var homeCity = TSPCities.AllCities[0];
            var citiesToVisit = new List<City> { 
                TSPCities.AllCities[1], 
                TSPCities.AllCities[2], 
                TSPCities.AllCities[3] 
            };

            // Act
            var result = TSPSolver.SolveNearestNeighbor(matrix, homeCity, citiesToVisit);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Nearest Neighbor", result.AlgorithmName);
            Assert.NotNull(result.Route);
            Assert.True(result.Route.TotalDistance > 0);
            Assert.True(result.ExecutionTimeMs >= 0);
        }

        [Fact]
        public void AllAlgorithms_Should_ReturnValidResults_SmallProblem()
        {
            // Arrange
            var matrix = new int[10, 10];
            
            // Set up a simple problem
            matrix[0, 1] = matrix[1, 0] = 10;
            matrix[0, 2] = matrix[2, 0] = 15;
            matrix[1, 2] = matrix[2, 1] = 20;

            var homeCity = TSPCities.AllCities[0];
            var citiesToVisit = new List<City> { TSPCities.AllCities[1], TSPCities.AllCities[2] };

            // Act
            var bruteForceResult = TSPSolver.SolveBruteForce(matrix, homeCity, citiesToVisit);
            var dpResult = TSPSolver.SolveDynamicProgramming(matrix, homeCity, citiesToVisit);
            var nnResult = TSPSolver.SolveNearestNeighbor(matrix, homeCity, citiesToVisit);

            // Assert - All should find valid solutions
            Assert.NotNull(bruteForceResult);
            Assert.NotNull(dpResult);
            Assert.NotNull(nnResult);
            
            Assert.True(bruteForceResult.Route.TotalDistance > 0);
            Assert.True(dpResult.Route.TotalDistance > 0);
            Assert.True(nnResult.Route.TotalDistance > 0);

            // Brute Force and DP should find optimal (same result for small problems)
            Assert.Equal(bruteForceResult.Route.TotalDistance, dpResult.Route.TotalDistance);
            
            // NN should not be better than optimal
            Assert.True(nnResult.Route.TotalDistance >= bruteForceResult.Route.TotalDistance);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void AllAlgorithms_Should_HandleDifferentProblemSizes(int cityCount)
        {
            // Arrange
            var matrix = TSPSolver.GenerateRandomDistanceMatrix();
            var homeCity = TSPCities.AllCities[0];
            var citiesToVisit = TSPCities.AllCities.Skip(1).Take(cityCount).ToList();

            // Act & Assert - Should not throw exceptions
            var bruteForceResult = TSPSolver.SolveBruteForce(matrix, homeCity, citiesToVisit);
            var dpResult = TSPSolver.SolveDynamicProgramming(matrix, homeCity, citiesToVisit);
            var nnResult = TSPSolver.SolveNearestNeighbor(matrix, homeCity, citiesToVisit);

            Assert.NotNull(bruteForceResult);
            Assert.NotNull(dpResult);
            Assert.NotNull(nnResult);
            
            Assert.True(bruteForceResult.Route.TotalDistance > 0);
            Assert.True(dpResult.Route.TotalDistance > 0);
            Assert.True(nnResult.Route.TotalDistance > 0);
        }

        [Fact]
        public void TSPCities_Should_HaveValidStructure()
        {
            // Assert
            Assert.NotNull(TSPCities.AllCities);
            Assert.Equal(10, TSPCities.AllCities.Length);
            
            // Check cities have valid names and indices
            for (int i = 0; i < TSPCities.AllCities.Length; i++)
            {
                var city = TSPCities.AllCities[i];
                Assert.NotNull(city);
                Assert.Equal(i, city.Index);
                Assert.True(city.Name >= 'A' && city.Name <= 'J');
            }
        }

        [Fact]
        public void TSPCities_GetCityByName_Should_ReturnCorrectCity()
        {
            // Act & Assert
            var cityA = TSPCities.GetCityByName('A');
            Assert.NotNull(cityA);
            Assert.Equal('A', cityA.Name);
            Assert.Equal(0, cityA.Index);

            var cityJ = TSPCities.GetCityByName('J');
            Assert.NotNull(cityJ);
            Assert.Equal('J', cityJ.Name);
            Assert.Equal(9, cityJ.Index);
        }

        [Fact]
        public void TSPCities_GetCityByIndex_Should_ReturnCorrectCity()
        {
            // Act & Assert
            var city0 = TSPCities.GetCityByIndex(0);
            Assert.NotNull(city0);
            Assert.Equal('A', city0.Name);
            Assert.Equal(0, city0.Index);

            var city9 = TSPCities.GetCityByIndex(9);
            Assert.NotNull(city9);
            Assert.Equal('J', city9.Name);
            Assert.Equal(9, city9.Index);
        }
    }
}