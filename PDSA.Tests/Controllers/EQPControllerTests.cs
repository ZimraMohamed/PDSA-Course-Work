using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PDSA.API.Controllers.Games;
using PDSA.API.Data;
using PDSA.Core.Algorithms.EightQueens;

namespace PDSA.Tests.Controllers
{
    public class EQPControllerTests : IDisposable
    {
        private readonly PDSADbContext _context;
        private readonly Mock<ILogger<EQPController>> _loggerMock;
        private readonly EQPController _controller;

        public EQPControllerTests()
        {
            var options = new DbContextOptionsBuilder<PDSADbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new PDSADbContext(options);
            _loggerMock = new Mock<ILogger<EQPController>>();
            _controller = new EQPController(_loggerMock.Object, _context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private async Task SeedAllSolutions()
        {
            // Generate all 92 solutions using the EQP solver
            var (solutions, _) = EQPSolver.SolveSequential(8);
            
            foreach (var solution in solutions)
            {
                var positions = new List<QueenPosition>();
                for (int row = 0; row < solution.Length; row++)
                {
                    positions.Add(new QueenPosition { Row = row, Col = solution[row] });
                }
                
                var solutionText = string.Join(",", positions.OrderBy(p => p.Row).Select(p => $"{p.Row}-{p.Col}"));
                
                var eqpSolution = new PDSA.API.Data.Models.EightQueens.EQPSolution
                {
                    Solution_Text = solutionText,
                    IsFound = false
                };
                
                _context.EQPSolutions.Add(eqpSolution);
            }
            
            await _context.SaveChangesAsync();
        }

        [Fact]
        public void CreateNewGame_Should_ReturnOkResult()
        {
            // Act
            var result = _controller.CreateNewGame();

            // Assert
            var okResult = Assert.IsType<ActionResult<EQPGameRoundDto>>(result);
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void CreateNewGame_Should_ReturnValidGameRound()
        {
            // Act
            var result = _controller.CreateNewGame();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<EQPGameRoundDto>(okResult.Value);
            
            Assert.NotEqual(Guid.Empty, dto.GameId);
            Assert.Equal(8, dto.BoardSize);
        }

        [Fact]
        public async Task Solve_Should_ReturnBadRequest_ForEmptyGameId()
        {
            // Arrange
            var request = new EQPSolveRequest { GameId = Guid.Empty };

            // Act
            var result = await _controller.Solve(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Solve_Should_ReturnError_ForNonExistentGame()
        {
            // Arrange
            var request = new EQPSolveRequest { GameId = Guid.NewGuid() };

            // Act
            var result = await _controller.Solve(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task Solve_Should_SaveAlgorithmTimesToDatabase()
        {
            // Arrange
            var gameRound = EQPGameService.CreateNewGameRound();
            var request = new EQPSolveRequest { GameId = gameRound.GameId };

            // Act
            var result = await _controller.Solve(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<EQPSolveResultDto>(okResult.Value);
            
            // Check database was updated
            var savedTimes = await _context.EQPAlgoTimes.ToListAsync();
            Assert.NotEmpty(savedTimes);
            Assert.Equal(2, savedTimes.Count); // Sequential and Threaded
        }

        [Fact]
        public async Task Solve_Should_ReturnBothAlgorithmResults()
        {
            // Arrange
            var gameRound = EQPGameService.CreateNewGameRound();
            var request = new EQPSolveRequest { GameId = gameRound.GameId };

            // Act
            var result = await _controller.Solve(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<EQPSolveResultDto>(okResult.Value);
            
            Assert.Equal(2, dto.AlgorithmResults.Count);
            Assert.Contains(dto.AlgorithmResults, ar => ar.AlgorithmName.Contains("Sequential"));
            Assert.Contains(dto.AlgorithmResults, ar => ar.AlgorithmName.Contains("Threaded"));
        }

        [Fact]
        public async Task Solve_Should_Return92Solutions_For8Queens()
        {
            // Arrange
            var gameRound = EQPGameService.CreateNewGameRound(8);
            var request = new EQPSolveRequest { GameId = gameRound.GameId };

            // Act
            var result = await _controller.Solve(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<EQPSolveResultDto>(okResult.Value);
            
            Assert.Equal(92, dto.TotalSolutionsSequential);
            Assert.Equal(92, dto.TotalSolutionsThreaded);
        }

        [Fact]
        public async Task SubmitSolution_Should_ReturnOkResult_ForValidSolution()
        {
            // Arrange
            await SeedAllSolutions(); // Seed all 92 solutions first
            
            var gameRound = EQPGameService.CreateNewGameRound();
            var validPositions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 1, Col = 4 },
                new QueenPosition { Row = 2, Col = 7 },
                new QueenPosition { Row = 3, Col = 5 },
                new QueenPosition { Row = 4, Col = 2 },
                new QueenPosition { Row = 5, Col = 6 },
                new QueenPosition { Row = 6, Col = 1 },
                new QueenPosition { Row = 7, Col = 3 }
            };

            var request = new EQPSubmitRequest
            {
                GameId = gameRound.GameId,
                PlayerName = "TestPlayer",
                QueenPositions = validPositions
            };

            // Act
            var result = await _controller.SubmitSolution(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<EQPSolutionSubmissionResultDto>(okResult.Value);
            Assert.True(dto.IsCorrect);
        }

        [Fact]
        public async Task SubmitSolution_Should_ReturnInvalid_ForInvalidSolution()
        {
            // Arrange
            var gameRound = EQPGameService.CreateNewGameRound();
            var invalidPositions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 1, Col = 1 }, // Diagonal attack
                new QueenPosition { Row = 2, Col = 2 },
                new QueenPosition { Row = 3, Col = 3 },
                new QueenPosition { Row = 4, Col = 4 },
                new QueenPosition { Row = 5, Col = 5 },
                new QueenPosition { Row = 6, Col = 6 },
                new QueenPosition { Row = 7, Col = 7 }
            };

            var request = new EQPSubmitRequest
            {
                GameId = gameRound.GameId,
                PlayerName = "TestPlayer",
                QueenPositions = invalidPositions
            };

            // Act
            var result = await _controller.SubmitSolution(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<EQPSolutionSubmissionResultDto>(okResult.Value);
            Assert.False(dto.IsCorrect);
        }

        [Fact]
        public async Task SubmitSolution_Should_SaveToDatabase_WhenValid()
        {
            // Arrange
            await SeedAllSolutions(); // Seed all 92 solutions first
            
            var gameRound = EQPGameService.CreateNewGameRound();
            var validPositions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 1, Col = 4 },
                new QueenPosition { Row = 2, Col = 7 },
                new QueenPosition { Row = 3, Col = 5 },
                new QueenPosition { Row = 4, Col = 2 },
                new QueenPosition { Row = 5, Col = 6 },
                new QueenPosition { Row = 6, Col = 1 },
                new QueenPosition { Row = 7, Col = 3 }
            };

            var request = new EQPSubmitRequest
            {
                GameId = gameRound.GameId,
                PlayerName = "TestPlayer",
                QueenPositions = validPositions
            };

            // Act
            await _controller.SubmitSolution(request);

            // Assert
            var savedSolutions = await _context.EQPSolutions.Where(s => s.IsFound).ToListAsync();
            Assert.NotEmpty(savedSolutions); // At least one solution should be marked as found
            
            var player = await _context.Players.FirstOrDefaultAsync(p => p.Name == "TestPlayer");
            Assert.NotNull(player);
        }

        [Fact]
        public async Task Solve_Should_IncludeExecutionTimes()
        {
            // Arrange
            var gameRound = EQPGameService.CreateNewGameRound();
            var request = new EQPSolveRequest { GameId = gameRound.GameId };

            // Act
            var result = await _controller.Solve(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<EQPSolveResultDto>(okResult.Value);
            
            Assert.All(dto.AlgorithmResults, ar => Assert.True(ar.ExecutionTimeMs >= 0));
        }

        [Fact]
        public async Task Solve_Should_AssignRoundNumber()
        {
            // Arrange
            var gameRound = EQPGameService.CreateNewGameRound();
            var request = new EQPSolveRequest { GameId = gameRound.GameId };

            // Act
            var result = await _controller.Solve(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<EQPSolveResultDto>(okResult.Value);
            
            Assert.True(dto.RoundNumber > 0);
        }

        [Fact]
        public async Task SubmitSolution_Should_ReturnBadRequest_ForEmptyQueenPositions()
        {
            // Arrange
            var request = new EQPSubmitRequest
            {
                GameId = Guid.NewGuid(),
                PlayerName = "TestPlayer",
                QueenPositions = null // Missing queen positions
            };

            // Act
            var result = await _controller.SubmitSolution(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task Solve_Should_HandleMultipleRounds()
        {
            // Arrange
            var gameRound1 = EQPGameService.CreateNewGameRound();
            var gameRound2 = EQPGameService.CreateNewGameRound();
            var request1 = new EQPSolveRequest { GameId = gameRound1.GameId };
            var request2 = new EQPSolveRequest { GameId = gameRound2.GameId };

            // Act
            var result1 = await _controller.Solve(request1);
            var result2 = await _controller.Solve(request2);

            // Assert
            var dto1 = Assert.IsType<EQPSolveResultDto>(((OkObjectResult)result1.Result!).Value);
            var dto2 = Assert.IsType<EQPSolveResultDto>(((OkObjectResult)result2.Result!).Value);
            
            Assert.NotEqual(dto1.RoundNumber, dto2.RoundNumber);
        }
    }
}
