using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PDSA.API.Controllers.Games;
using PDSA.API.Data;
using PDSA.Core.Algorithms.TravelingSalesman;

namespace PDSA.Tests.Controllers
{
    public class TSPControllerTests : IDisposable
    {
        private readonly PDSADbContext _context;
        private readonly Mock<ILogger<TSPController>> _loggerMock;
        private readonly TSPController _controller;

        public TSPControllerTests()
        {
            var options = new DbContextOptionsBuilder<PDSADbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new PDSADbContext(options);
            _loggerMock = new Mock<ILogger<TSPController>>();
            _controller = new TSPController(_loggerMock.Object, _context);
        }

        private int[][] CreateJaggedArray(int rows, int cols)
        {
            var matrix = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                matrix[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    matrix[i][j] = (i == j) ? 0 : Math.Abs(i - j) * 10;
                }
            }
            return matrix;
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void CreateNewGame_Should_ReturnOkResult()
        {
            // Act
            var result = _controller.CreateNewGame();

            // Assert
            var okResult = Assert.IsType<ActionResult<TSPGameRoundDto>>(result);
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void CreateNewGame_Should_ReturnGameRoundWithValidData()
        {
            // Act
            var result = _controller.CreateNewGame();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TSPGameRoundDto>(okResult.Value);
            
            Assert.NotEqual(Guid.Empty, dto.GameId);
            Assert.InRange(dto.HomeCityIndex, 0, 9);
            Assert.NotNull(dto.DistanceMatrix);
            Assert.NotNull(dto.AllCities);
            Assert.Equal(10, dto.AllCities.Length); // A-J cities
        }

        [Fact]
        public void CreateNewGame_Should_ReturnValidDistanceMatrix()
        {
            // Act
            var result = _controller.CreateNewGame();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TSPGameRoundDto>(okResult.Value);
            
            Assert.Equal(10, dto.DistanceMatrix.Length);
            Assert.All(dto.DistanceMatrix, row => Assert.Equal(10, row.Length));
        }

        [Fact]
        public void SolveTSP_Should_ReturnBadRequest_ForInvalidCities()
        {
            // Arrange
            var distanceMatrix = CreateJaggedArray(10, 10);
            
            var request = new TSPSolveRequest
            {
                GameId = Guid.NewGuid(),
                SelectedCities = new List<char> { 'A' }, // Only 1 city - invalid
                HomeCityName = 'A',
                DistanceMatrix = distanceMatrix
            };

            // Act
            var result = _controller.SolveTSP(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Invalid city selection", badRequestResult.Value?.ToString());
        }

        [Fact]
        public void SolveTSP_Should_ReturnOkResult_ForValidRequest()
        {
            // Arrange
            var distanceMatrix = CreateJaggedArray(10, 10);

            var request = new TSPSolveRequest
            {
                GameId = Guid.NewGuid(),
                SelectedCities = new List<char> { 'A', 'B', 'C' },
                HomeCityName = 'A',
                DistanceMatrix = distanceMatrix
            };

            // Act
            var result = _controller.SolveTSP(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TSPSolutionDto>(okResult.Value);
            Assert.NotNull(dto);
        }

        [Fact]
        public void SolveTSP_Should_ReturnMultipleAlgorithmResults()
        {
            // Arrange
            var distanceMatrix = CreateJaggedArray(10, 10);

            var request = new TSPSolveRequest
            {
                GameId = Guid.NewGuid(),
                SelectedCities = new List<char> { 'A', 'B', 'C' },
                HomeCityName = 'A',
                DistanceMatrix = distanceMatrix
            };

            // Act
            var result = _controller.SolveTSP(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TSPSolutionDto>(okResult.Value);
            Assert.NotEmpty(dto.AlgorithmResults);
            Assert.True(dto.AlgorithmResults.Count >= 2); // At least BruteForce and DP
        }

        [Fact]
        public void SolveTSP_Should_IncludeComplexityAnalysis()
        {
            // Arrange
            var distanceMatrix = CreateJaggedArray(10, 10);

            var request = new TSPSolveRequest
            {
                GameId = Guid.NewGuid(),
                SelectedCities = new List<char> { 'A', 'B', 'C', 'D' },
                HomeCityName = 'A',
                DistanceMatrix = distanceMatrix
            };

            // Act
            var result = _controller.SolveTSP(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TSPSolutionDto>(okResult.Value);
            Assert.NotNull(dto.ComplexityAnalysis);
        }

        [Fact]
        public void SolveTSP_Should_ReturnOptimalDistance()
        {
            // Arrange
            var distanceMatrix = CreateJaggedArray(10, 10);

            var request = new TSPSolveRequest
            {
                GameId = Guid.NewGuid(),
                SelectedCities = new List<char> { 'A', 'B', 'C' },
                HomeCityName = 'A',
                DistanceMatrix = distanceMatrix
            };

            // Act
            var result = _controller.SolveTSP(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TSPSolutionDto>(okResult.Value);
            Assert.True(dto.OptimalDistance > 0);
        }

        [Fact]
        public void SolveTSP_Should_ReturnOptimalRoute()
        {
            // Arrange
            var distanceMatrix = CreateJaggedArray(10, 10);

            var request = new TSPSolveRequest
            {
                GameId = Guid.NewGuid(),
                SelectedCities = new List<char> { 'A', 'B', 'C' },
                HomeCityName = 'A',
                DistanceMatrix = distanceMatrix
            };

            // Act
            var result = _controller.SolveTSP(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TSPSolutionDto>(okResult.Value);
            Assert.NotNull(dto.OptimalRoute);
            Assert.NotEmpty(dto.OptimalRoute);
        }

        [Fact]
        public void SolveTSP_Should_IncludeExecutionTimes()
        {
            // Arrange
            var distanceMatrix = CreateJaggedArray(10, 10);

            var request = new TSPSolveRequest
            {
                GameId = Guid.NewGuid(),
                SelectedCities = new List<char> { 'A', 'B', 'C' },
                HomeCityName = 'A',
                DistanceMatrix = distanceMatrix
            };

            // Act
            var result = _controller.SolveTSP(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TSPSolutionDto>(okResult.Value);
            Assert.All(dto.AlgorithmResults, ar => Assert.True(ar.ExecutionTimeMs >= 0));
        }

        [Fact]
        public void CreateNewGame_Should_IncludeAllCities()
        {
            // Act
            var result = _controller.CreateNewGame();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TSPGameRoundDto>(okResult.Value);
            
            var cityNames = dto.AllCities.Select(c => c.Name).ToList();
            Assert.Contains('A', cityNames);
            Assert.Contains('J', cityNames);
        }

        [Fact]
        public void SolveTSP_Should_ReturnRouteWithHomeCityFirst()
        {
            // Arrange
            var distanceMatrix = CreateJaggedArray(10, 10);

            var request = new TSPSolveRequest
            {
                GameId = Guid.NewGuid(),
                SelectedCities = new List<char> { 'A', 'B', 'C' },
                HomeCityName = 'A',
                DistanceMatrix = distanceMatrix
            };

            // Act
            var result = _controller.SolveTSP(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TSPSolutionDto>(okResult.Value);
            Assert.Equal('A', dto.OptimalRoute[0]);
        }

        [Fact]
        public void SolveTSP_Should_HandleLargerCitySelection()
        {
            // Arrange
            var distanceMatrix = CreateJaggedArray(10, 10);

            var request = new TSPSolveRequest
            {
                GameId = Guid.NewGuid(),
                SelectedCities = new List<char> { 'A', 'B', 'C', 'D', 'E' },
                HomeCityName = 'A',
                DistanceMatrix = distanceMatrix
            };

            // Act
            var result = _controller.SolveTSP(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TSPSolutionDto>(okResult.Value);
            Assert.NotNull(dto);
            Assert.True(dto.OptimalDistance > 0);
        }

        [Fact]
        public void SolveTSP_Should_ReturnBadRequest_ForEmptyCities()
        {
            // Arrange
            var distanceMatrix = CreateJaggedArray(10, 10);
            
            var request = new TSPSolveRequest
            {
                GameId = Guid.NewGuid(),
                SelectedCities = new List<char>(),
                HomeCityName = 'A',
                DistanceMatrix = distanceMatrix
            };

            // Act
            var result = _controller.SolveTSP(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void CreateNewGame_Should_HaveSymmetricDistanceMatrix()
        {
            // Act
            var result = _controller.CreateNewGame();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TSPGameRoundDto>(okResult.Value);
            
            // Check symmetry: distance[i][j] should equal distance[j][i]
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Assert.Equal(dto.DistanceMatrix[i][j], dto.DistanceMatrix[j][i]);
                }
            }
        }

        [Fact]
        public void CreateNewGame_Should_HaveZeroDiagonal()
        {
            // Act
            var result = _controller.CreateNewGame();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TSPGameRoundDto>(okResult.Value);
            
            // Check diagonal: distance from city to itself should be 0
            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(0, dto.DistanceMatrix[i][i]);
            }
        }

        [Fact]
        public void SolveTSP_Should_IncludeDistanceMatrixDisplay()
        {
            // Arrange
            var distanceMatrix = CreateJaggedArray(10, 10);

            var request = new TSPSolveRequest
            {
                GameId = Guid.NewGuid(),
                SelectedCities = new List<char> { 'A', 'B', 'C' },
                HomeCityName = 'A',
                DistanceMatrix = distanceMatrix
            };

            // Act
            var result = _controller.SolveTSP(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TSPSolutionDto>(okResult.Value);
            Assert.NotNull(dto.DistanceMatrixDisplay);
        }
    }
}
