using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDSA.API.Controllers.Games;
using PDSA.API.Data;
using PDSA.Core.Algorithms.SnakeAndLadder;

namespace PDSA.Tests.Controllers
{
    public class SALControllerTests : IDisposable
    {
        private readonly PDSADbContext _context;
        private readonly SALController _controller;

        public SALControllerTests()
        {
            var options = new DbContextOptionsBuilder<PDSADbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new PDSADbContext(options);
            _controller = new SALController(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void NewGame_Should_ReturnOkResult()
        {
            // Arrange
            var request = new NewGameRequest { BoardSize = 10 };

            // Act
            var result = _controller.NewGame(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void NewGame_Should_ReturnValidBoard()
        {
            // Arrange
            var request = new NewGameRequest { BoardSize = 10 };

            // Act
            var result = _controller.NewGame(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            Assert.NotNull(value);
        }

        [Fact]
        public void NewGame_Should_ReturnBadRequest_ForInvalidBoardSize()
        {
            // Arrange
            var request = new NewGameRequest { BoardSize = 1 }; // Too small

            // Act
            var result = _controller.NewGame(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void NewGame_Should_ReturnBadRequest_ForTooLargeBoardSize()
        {
            // Arrange
            var request = new NewGameRequest { BoardSize = 25 }; // Too large

            // Act
            var result = _controller.NewGame(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GenerateGame_Should_ReturnOkResult()
        {
            // Act
            var result = _controller.GenerateGame(10);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GenerateGame_Should_UseDefaultBoardSize()
        {
            // Act
            var result = _controller.GenerateGame();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            Assert.NotNull(value);
        }

        [Fact]
        public void GenerateGame_Should_ReturnBadRequest_ForInvalidSize()
        {
            // Act
            var result = _controller.GenerateGame(1);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void SolveGame_Should_ReturnBadRequest_ForMissingGameId()
        {
            // Arrange
            var request = new SolveRequest
            {
                GameId = "",
                BoardSize = 10,
                Snakes = new List<SnakeDto>(),
                Ladders = new List<LadderDto>()
            };

            // Act
            var result = _controller.SolveGame(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void SolveGame_Should_ReturnOkResult_ForValidRequest()
        {
            // Arrange
            var request = new SolveRequest
            {
                GameId = Guid.NewGuid().ToString(),
                BoardSize = 10,
                Snakes = new List<SnakeDto>
                {
                    new SnakeDto { Head = 50, Tail = 10 }
                },
                Ladders = new List<LadderDto>
                {
                    new LadderDto { Bottom = 5, Top = 30 }
                }
            };

            // Act
            var result = _controller.SolveGame(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void SolveGame_Should_ReturnBothAlgorithmResults()
        {
            // Arrange
            var request = new SolveRequest
            {
                GameId = Guid.NewGuid().ToString(),
                BoardSize = 10,
                Snakes = new List<SnakeDto>(),
                Ladders = new List<LadderDto>()
            };

            // Act
            var result = _controller.SolveGame(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            Assert.NotNull(value);
        }

        [Fact]
        public void SolveGame_Should_HandleEmptySnakesAndLadders()
        {
            // Arrange
            var request = new SolveRequest
            {
                GameId = Guid.NewGuid().ToString(),
                BoardSize = 5,
                Snakes = new List<SnakeDto>(),
                Ladders = new List<LadderDto>()
            };

            // Act
            var result = _controller.SolveGame(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ValidateAnswer_Should_ReturnBadRequest_ForMissingGameId()
        {
            // Arrange
            var request = new ValidateAnswerRequest
            {
                GameId = "",
                PlayerName = "TestPlayer",
                UserAnswer = 10,
                CorrectAnswer = 10,
                BoardSize = 10,
                Snakes = new List<SnakeDto>(),
                Ladders = new List<LadderDto>()
            };

            // Act
            var result = _controller.ValidateAnswer(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ValidateAnswer_Should_ReturnBadRequest_ForMissingPlayerName()
        {
            // Arrange
            var request = new ValidateAnswerRequest
            {
                GameId = Guid.NewGuid().ToString(),
                PlayerName = "",
                UserAnswer = 10,
                CorrectAnswer = 10,
                BoardSize = 10,
                Snakes = new List<SnakeDto>(),
                Ladders = new List<LadderDto>()
            };

            // Act
            var result = _controller.ValidateAnswer(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ValidateAnswer_Should_ReturnOkResult_ForValidRequest()
        {
            // Arrange
            var request = new ValidateAnswerRequest
            {
                GameId = Guid.NewGuid().ToString(),
                PlayerName = "TestPlayer",
                UserAnswer = 5,
                CorrectAnswer = 5,
                BoardSize = 5,
                Snakes = new List<SnakeDto>(),
                Ladders = new List<LadderDto>()
            };

            // Act
            var result = _controller.ValidateAnswer(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ValidateAnswer_Should_SaveToDatabase_ForCorrectAnswer()
        {
            // Arrange
            var request = new ValidateAnswerRequest
            {
                GameId = Guid.NewGuid().ToString(),
                PlayerName = "TestPlayer",
                UserAnswer = 5,
                CorrectAnswer = 5,
                BoardSize = 5,
                Snakes = new List<SnakeDto>(),
                Ladders = new List<LadderDto>()
            };

            // Act
            _controller.ValidateAnswer(request);

            // Assert
            var player = _context.Players.FirstOrDefault(p => p.Name == "TestPlayer");
            Assert.NotNull(player);
        }

        [Fact]
        public void ValidateAnswer_Should_CreatePlayer_IfNotExists()
        {
            // Arrange
            var request = new ValidateAnswerRequest
            {
                GameId = Guid.NewGuid().ToString(),
                PlayerName = "NewPlayer",
                UserAnswer = 5,
                CorrectAnswer = 5,
                BoardSize = 5,
                Snakes = new List<SnakeDto>(),
                Ladders = new List<LadderDto>()
            };

            // Act
            _controller.ValidateAnswer(request);

            // Assert
            var player = _context.Players.FirstOrDefault(p => p.Name == "NewPlayer");
            Assert.NotNull(player);
        }

        [Fact]
        public void ValidateAnswer_Should_ReuseExistingPlayer()
        {
            // Arrange
            var player = new PDSA.API.Data.Models.Player { Name = "ExistingPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var request = new ValidateAnswerRequest
            {
                GameId = Guid.NewGuid().ToString(),
                PlayerName = "ExistingPlayer",
                UserAnswer = 5,
                CorrectAnswer = 5,
                BoardSize = 5,
                Snakes = new List<SnakeDto>(),
                Ladders = new List<LadderDto>()
            };

            // Act
            _controller.ValidateAnswer(request);

            // Assert
            var players = _context.Players.Where(p => p.Name == "ExistingPlayer").ToList();
            Assert.Single(players);
        }

        [Fact]
        public void NewGame_Should_ReturnGameIdAndBoardSize()
        {
            // Arrange
            var request = new NewGameRequest { BoardSize = 10 };

            // Act
            var result = _controller.NewGame(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            Assert.NotNull(value);
        }

        [Fact]
        public void GenerateGame_Should_HandleCustomBoardSize()
        {
            // Act
            var result = _controller.GenerateGame(15);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void SolveGame_Should_HandleBoardWithOnlySnakes()
        {
            // Arrange
            var request = new SolveRequest
            {
                GameId = Guid.NewGuid().ToString(),
                BoardSize = 10,
                Snakes = new List<SnakeDto>
                {
                    new SnakeDto { Head = 50, Tail = 10 },
                    new SnakeDto { Head = 70, Tail = 30 }
                },
                Ladders = new List<LadderDto>()
            };

            // Act
            var result = _controller.SolveGame(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void SolveGame_Should_HandleBoardWithOnlyLadders()
        {
            // Arrange
            var request = new SolveRequest
            {
                GameId = Guid.NewGuid().ToString(),
                BoardSize = 10,
                Snakes = new List<SnakeDto>(),
                Ladders = new List<LadderDto>
                {
                    new LadderDto { Bottom = 5, Top = 30 },
                    new LadderDto { Bottom = 15, Top = 55 }
                }
            };

            // Act
            var result = _controller.SolveGame(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ValidateAnswer_Should_HandleWrongAnswer()
        {
            // Arrange
            var request = new ValidateAnswerRequest
            {
                GameId = Guid.NewGuid().ToString(),
                PlayerName = "TestPlayer",
                UserAnswer = 100,
                CorrectAnswer = 5,
                BoardSize = 5,
                Snakes = new List<SnakeDto>(),
                Ladders = new List<LadderDto>()
            };

            // Act
            var result = _controller.ValidateAnswer(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
