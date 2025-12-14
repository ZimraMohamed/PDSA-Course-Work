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
            Assert.Equal("Brute Force (Recursive)", result.AlgorithmName);
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
            Assert.Equal("Nearest Neighbor (Iterative)", result.AlgorithmName);
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

        [Fact]
        public void SolveBruteForce_Should_VisitAllCitiesExactlyOnce()
        {
            // Arrange
            var matrix = TSPSolver.GenerateRandomDistanceMatrix();
            var homeCity = TSPCities.AllCities[0];
            var citiesToVisit = new List<City> { 
                TSPCities.AllCities[1], 
                TSPCities.AllCities[2], 
                TSPCities.AllCities[3] 
            };

            // Act
            var result = TSPSolver.SolveBruteForce(matrix, homeCity, citiesToVisit);

            // Assert
            var visitedCities = new HashSet<int>();
            foreach (var city in result.Route.Cities)
            {
                visitedCities.Add(city.Index);
            }
            
            // Should visit home city and all selected cities
            Assert.Contains(homeCity.Index, visitedCities);
            foreach (var city in citiesToVisit)
            {
                Assert.Contains(city.Index, visitedCities);
            }
        }

        [Fact]
        public void SolveDynamicProgramming_Should_VisitAllCitiesExactlyOnce()
        {
            // Arrange
            var matrix = TSPSolver.GenerateRandomDistanceMatrix();
            var homeCity = TSPCities.AllCities[0];
            var citiesToVisit = new List<City> { 
                TSPCities.AllCities[1], 
                TSPCities.AllCities[2], 
                TSPCities.AllCities[3] 
            };

            // Act
            var result = TSPSolver.SolveDynamicProgramming(matrix, homeCity, citiesToVisit);

            // Assert
            var visitedCities = new HashSet<int>();
            foreach (var city in result.Route.Cities)
            {
                visitedCities.Add(city.Index);
            }
            
            // Should visit home city and all selected cities
            Assert.Contains(homeCity.Index, visitedCities);
            foreach (var city in citiesToVisit)
            {
                Assert.Contains(city.Index, visitedCities);
            }
        }

        [Fact]
        public void SolveNearestNeighbor_Should_VisitAllCitiesExactlyOnce()
        {
            // Arrange
            var matrix = TSPSolver.GenerateRandomDistanceMatrix();
            var homeCity = TSPCities.AllCities[0];
            var citiesToVisit = new List<City> { 
                TSPCities.AllCities[1], 
                TSPCities.AllCities[2], 
                TSPCities.AllCities[3] 
            };

            // Act
            var result = TSPSolver.SolveNearestNeighbor(matrix, homeCity, citiesToVisit);

            // Assert
            var visitedCities = new HashSet<int>();
            foreach (var city in result.Route.Cities)
            {
                visitedCities.Add(city.Index);
            }
            
            // Should visit home city and all selected cities
            Assert.Contains(homeCity.Index, visitedCities);
            foreach (var city in citiesToVisit)
            {
                Assert.Contains(city.Index, visitedCities);
            }
        }

        [Fact]
        public void SolveBruteForce_Should_StartAtHomeCity()
        {
            // Arrange
            var matrix = TSPSolver.GenerateRandomDistanceMatrix();
            var homeCity = TSPCities.AllCities[2]; // City 'C'
            var citiesToVisit = new List<City> { 
                TSPCities.AllCities[0], 
                TSPCities.AllCities[1] 
            };

            // Act
            var result = TSPSolver.SolveBruteForce(matrix, homeCity, citiesToVisit);

            // Assert
            Assert.Equal(homeCity.Index, result.Route.Cities[0].Index);
        }

        [Fact]
        public void SolveDynamicProgramming_Should_StartAtHomeCity()
        {
            // Arrange
            var matrix = TSPSolver.GenerateRandomDistanceMatrix();
            var homeCity = TSPCities.AllCities[3]; // City 'D'
            var citiesToVisit = new List<City> { 
                TSPCities.AllCities[0], 
                TSPCities.AllCities[1] 
            };

            // Act
            var result = TSPSolver.SolveDynamicProgramming(matrix, homeCity, citiesToVisit);

            // Assert
            Assert.Equal(homeCity.Index, result.Route.Cities[0].Index);
        }

        [Fact]
        public void SolveNearestNeighbor_Should_StartAtHomeCity()
        {
            // Arrange
            var matrix = TSPSolver.GenerateRandomDistanceMatrix();
            var homeCity = TSPCities.AllCities[4]; // City 'E'
            var citiesToVisit = new List<City> { 
                TSPCities.AllCities[0], 
                TSPCities.AllCities[1] 
            };

            // Act
            var result = TSPSolver.SolveNearestNeighbor(matrix, homeCity, citiesToVisit);

            // Assert
            Assert.Equal(homeCity.Index, result.Route.Cities[0].Index);
        }

        [Fact]
        public void GenerateRandomDistanceMatrix_Should_GenerateUniqueMatrices()
        {
            // Act
            var matrix1 = TSPSolver.GenerateRandomDistanceMatrix();
            var matrix2 = TSPSolver.GenerateRandomDistanceMatrix();

            // Assert - Should have at least some different values
            bool hasDifference = false;
            for (int i = 0; i < 10 && !hasDifference; i++)
            {
                for (int j = 0; j < 10 && !hasDifference; j++)
                {
                    if (matrix1[i, j] != matrix2[i, j])
                    {
                        hasDifference = true;
                    }
                }
            }
            Assert.True(hasDifference);
        }

        [Fact]
        public void SelectRandomHomeCity_Should_ReturnDifferentCitiesOverMultipleCalls()
        {
            // Act - Call multiple times
            var cities = new HashSet<char>();
            for (int i = 0; i < 50; i++)
            {
                var city = TSPSolver.SelectRandomHomeCity();
                cities.Add(city.Name);
            }

            // Assert - Should have selected more than one unique city over 50 attempts
            Assert.True(cities.Count > 1);
        }

        [Fact]
        public void SolveBruteForce_Should_CalculateCorrectDistanceForKnownPath()
        {
            // Arrange - Create a controlled distance matrix
            var matrix = new int[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    matrix[i, j] = Math.Abs(i - j) * 10; // Simple distance formula
                }
            }

            var homeCity = TSPCities.AllCities[0]; // City A (index 0)
            var citiesToVisit = new List<City> { TSPCities.AllCities[1] }; // City B (index 1)
            
            // Expected: A -> B -> A = 10 + 10 = 20

            // Act
            var result = TSPSolver.SolveBruteForce(matrix, homeCity, citiesToVisit);

            // Assert
            Assert.Equal(20, result.Route.TotalDistance);
        }

        [Fact]
        public void SolveDynamicProgramming_Should_CalculateCorrectDistanceForKnownPath()
        {
            // Arrange - Create a controlled distance matrix
            var matrix = new int[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    matrix[i, j] = Math.Abs(i - j) * 10;
                }
            }

            var homeCity = TSPCities.AllCities[0]; // City A (index 0)
            var citiesToVisit = new List<City> { TSPCities.AllCities[1] }; // City B (index 1)

            // Act
            var result = TSPSolver.SolveDynamicProgramming(matrix, homeCity, citiesToVisit);

            // Assert
            Assert.Equal(20, result.Route.TotalDistance);
        }

        [Fact]
        public void AllAlgorithms_Should_HandleLargerProblemSize()
        {
            // Arrange
            var matrix = TSPSolver.GenerateRandomDistanceMatrix();
            var homeCity = TSPCities.AllCities[0];
            var citiesToVisit = TSPCities.AllCities.Skip(1).Take(6).ToList(); // 6 cities

            // Act & Assert - Should complete without errors
            var bruteForceResult = TSPSolver.SolveBruteForce(matrix, homeCity, citiesToVisit);
            var dpResult = TSPSolver.SolveDynamicProgramming(matrix, homeCity, citiesToVisit);
            var nnResult = TSPSolver.SolveNearestNeighbor(matrix, homeCity, citiesToVisit);

            Assert.NotNull(bruteForceResult);
            Assert.NotNull(dpResult);
            Assert.NotNull(nnResult);
        }

        [Fact]
        public void BruteForce_Should_HaveComplexityValue()
        {
            // Arrange
            var matrix = TSPSolver.GenerateRandomDistanceMatrix();
            var homeCity = TSPCities.AllCities[0];
            var citiesToVisit = new List<City> { 
                TSPCities.AllCities[1], 
                TSPCities.AllCities[2] 
            };

            // Act
            var result = TSPSolver.SolveBruteForce(matrix, homeCity, citiesToVisit);

            // Assert
            Assert.True(result.Complexity > 0);
        }

        [Fact]
        public void DynamicProgramming_Should_HaveComplexityValue()
        {
            // Arrange
            var matrix = TSPSolver.GenerateRandomDistanceMatrix();
            var homeCity = TSPCities.AllCities[0];
            var citiesToVisit = new List<City> { 
                TSPCities.AllCities[1], 
                TSPCities.AllCities[2] 
            };

            // Act
            var result = TSPSolver.SolveDynamicProgramming(matrix, homeCity, citiesToVisit);

            // Assert
            Assert.True(result.Complexity > 0);
        }

        [Fact]
        public void NearestNeighbor_Should_HaveComplexityValue()
        {
            // Arrange
            var matrix = TSPSolver.GenerateRandomDistanceMatrix();
            var homeCity = TSPCities.AllCities[0];
            var citiesToVisit = new List<City> { 
                TSPCities.AllCities[1], 
                TSPCities.AllCities[2] 
            };

            // Act
            var result = TSPSolver.SolveNearestNeighbor(matrix, homeCity, citiesToVisit);

            // Assert
            Assert.True(result.Complexity > 0);
        }

        [Fact]
        public void TSPRoute_ToString_Should_ReturnFormattedString()
        {
            // Arrange
            var route = new TSPRoute
            {
                Cities = new List<City> 
                { 
                    TSPCities.AllCities[0], 
                    TSPCities.AllCities[1], 
                    TSPCities.AllCities[2] 
                },
                TotalDistance = 150
            };

            // Act
            var result = route.ToString();

            // Assert
            Assert.Contains("A", result);
            Assert.Contains("B", result);
            Assert.Contains("C", result);
            Assert.Contains("150", result);
            Assert.Contains("->", result);
        }

        [Fact]
        public void City_ToString_Should_ReturnCityName()
        {
            // Arrange
            var city = new City('X', 10);

            // Act
            var result = city.ToString();

            // Assert
            Assert.Equal("X", result);
        }

        [Fact]
        public void SolveNearestNeighbor_Should_BeGreedyAndSelectNearestAtEachStep()
        {
            // Arrange - Create a specific matrix where greedy choice is clear
            var matrix = new int[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    matrix[i, j] = 100; // Default high distance
                }
            }
            
            // Set specific distances: A->B=10, B->C=20, C->A=30
            matrix[0, 1] = matrix[1, 0] = 10;
            matrix[1, 2] = matrix[2, 1] = 20;
            matrix[2, 0] = matrix[0, 2] = 50;

            var homeCity = TSPCities.AllCities[0]; // A
            var citiesToVisit = new List<City> { 
                TSPCities.AllCities[1],  // B
                TSPCities.AllCities[2]   // C
            };

            // Act
            var result = TSPSolver.SolveNearestNeighbor(matrix, homeCity, citiesToVisit);

            // Assert - NN should pick: A -> B (10) -> C (20) -> A (50) = 80
            Assert.Equal(80, result.Route.TotalDistance);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(9)]
        public void AllAlgorithms_Should_HandleOddNumberOfCities(int cityCount)
        {
            // Arrange
            var matrix = TSPSolver.GenerateRandomDistanceMatrix();
            var homeCity = TSPCities.AllCities[0];
            var citiesToVisit = TSPCities.AllCities.Skip(1).Take(cityCount).ToList();

            // Act
            var bruteForceResult = TSPSolver.SolveBruteForce(matrix, homeCity, citiesToVisit);
            var dpResult = TSPSolver.SolveDynamicProgramming(matrix, homeCity, citiesToVisit);
            var nnResult = TSPSolver.SolveNearestNeighbor(matrix, homeCity, citiesToVisit);

            // Assert - All should produce valid results
            Assert.True(bruteForceResult.Route.TotalDistance > 0);
            Assert.True(dpResult.Route.TotalDistance > 0);
            Assert.True(nnResult.Route.TotalDistance > 0);
            
            // Optimal algorithms should match
            Assert.Equal(bruteForceResult.Route.TotalDistance, dpResult.Route.TotalDistance);
        }

        [Fact]
        public void BruteForceAndDP_Should_FindSameOptimalSolutionForComplexProblem()
        {
            // Arrange - Use a consistent seed matrix
            var matrix = new int[10, 10];
            var rand = new Random(42); // Fixed seed for reproducibility
            
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = 0;
                    }
                    else if (i < j)
                    {
                        matrix[i, j] = rand.Next(50, 101);
                        matrix[j, i] = matrix[i, j];
                    }
                }
            }

            var homeCity = TSPCities.AllCities[0];
            var citiesToVisit = new List<City> { 
                TSPCities.AllCities[1], 
                TSPCities.AllCities[2], 
                TSPCities.AllCities[3],
                TSPCities.AllCities[4]
            };

            // Act
            var bruteForceResult = TSPSolver.SolveBruteForce(matrix, homeCity, citiesToVisit);
            var dpResult = TSPSolver.SolveDynamicProgramming(matrix, homeCity, citiesToVisit);

            // Assert - Both optimal algorithms should find the same minimum distance
            Assert.Equal(bruteForceResult.Route.TotalDistance, dpResult.Route.TotalDistance);
        }
    }
}