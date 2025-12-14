using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDSA.API.Controllers.Games;
using PDSA.API.Data;
using PDSA.Core.Algorithms.TowerOfHanoi;

namespace PDSA.Tests.Controllers
{
    public class TOHPControllerTests : IDisposable
    {
        private readonly PDSADbContext _context;
        private readonly TOHPController _controller;

        public TOHPControllerTests()
        {
            var options = new DbContextOptionsBuilder<PDSADbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new PDSADbContext(options);
            _controller = new TOHPController(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void SubmitAnswer_Should_ReturnBadRequest_ForInvalidPegs()
        {
            // Arrange
            var request = new TOHPSubmitRequest
            {
                NumPegs = 0,
                NumDisks = 3,
                UserMovesCount = 7,
                UserSequence = "A->C",
                PlayerName = "TestPlayer"
            };

            // Act
            var result = _controller.SubmitAnswer(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void SubmitAnswer_Should_ReturnBadRequest_ForInvalidDisks()
        {
            // Arrange
            var request = new TOHPSubmitRequest
            {
                NumPegs = 3,
                NumDisks = 0,
                UserMovesCount = 0,
                UserSequence = "",
                PlayerName = "TestPlayer"
            };

            // Act
            var result = _controller.SubmitAnswer(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void SubmitAnswer_Should_ReturnBadRequest_ForEmptyPlayerName()
        {
            // Arrange
            var request = new TOHPSubmitRequest
            {
                NumPegs = 3,
                NumDisks = 3,
                UserMovesCount = 7,
                UserSequence = "A->C",
                PlayerName = ""
            };

            // Act
            var result = _controller.SubmitAnswer(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void SubmitAnswer_Should_ReturnOkResult_ForValidRequest()
        {
            // Arrange
            var correctSequence = "A->C, A->B, C->B, A->C, B->A, B->C, A->C";
            var request = new TOHPSubmitRequest
            {
                NumPegs = 3,
                NumDisks = 3,
                UserMovesCount = 7,
                UserSequence = correctSequence,
                PlayerName = "TestPlayer"
            };

            // Act
            var result = _controller.SubmitAnswer(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void SubmitAnswer_Should_SaveToDatabase_ForCorrectAnswer()
        {
            // Arrange
            var correctSequence = "A->C, A->B, C->B, A->C, B->A, B->C, A->C";
            var request = new TOHPSubmitRequest
            {
                NumPegs = 3,
                NumDisks = 3,
                UserMovesCount = 7,
                UserSequence = correctSequence,
                PlayerName = "TestPlayer"
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var player = _context.Players.FirstOrDefault(p => p.Name == "TestPlayer");
            Assert.NotNull(player);
            
            var round = _context.HanoiRounds.FirstOrDefault(r => r.PlayerID == player.PlayerID);
            Assert.NotNull(round);
            Assert.Equal(3, round.NumDisks_N);
            Assert.Equal(3, round.NumPegs);
        }

        [Fact]
        public void SubmitAnswer_Should_SaveAlgorithmTimes()
        {
            // Arrange
            var correctSequence = "A->C, A->B, C->B, A->C, B->A, B->C, A->C";
            var request = new TOHPSubmitRequest
            {
                NumPegs = 3,
                NumDisks = 3,
                UserMovesCount = 7,
                UserSequence = correctSequence,
                PlayerName = "TestPlayer"
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var algoTimes = _context.HanoiAlgoTimes.ToList();
            Assert.NotEmpty(algoTimes);
        }

        [Fact]
        public void SubmitAnswer_Should_NotSaveToDatabase_ForIncorrectMoves()
        {
            // Arrange
            var request = new TOHPSubmitRequest
            {
                NumPegs = 3,
                NumDisks = 3,
                UserMovesCount = 10, // Wrong count
                UserSequence = "A->C",
                PlayerName = "TestPlayer"
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var rounds = _context.HanoiRounds.ToList();
            Assert.Empty(rounds);
        }

        [Fact]
        public void SubmitAnswer_Should_NotSaveToDatabase_ForIncorrectSequence()
        {
            // Arrange
            var request = new TOHPSubmitRequest
            {
                NumPegs = 3,
                NumDisks = 3,
                UserMovesCount = 7,
                UserSequence = "A->B, B->A, A->C, C->A, A->B, B->A, A->C", // Wrong sequence
                PlayerName = "TestPlayer"
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var rounds = _context.HanoiRounds.ToList();
            Assert.Empty(rounds);
        }

        [Fact]
        public void SubmitAnswer_Should_CreatePlayer_IfNotExists()
        {
            // Arrange
            var correctSequence = "A->C, A->B, C->B, A->C, B->A, B->C, A->C";
            var request = new TOHPSubmitRequest
            {
                NumPegs = 3,
                NumDisks = 3,
                UserMovesCount = 7,
                UserSequence = correctSequence,
                PlayerName = "NewPlayer"
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var player = _context.Players.FirstOrDefault(p => p.Name == "NewPlayer");
            Assert.NotNull(player);
        }

        [Fact]
        public void SubmitAnswer_Should_ReuseExistingPlayer()
        {
            // Arrange
            var player = new PDSA.API.Data.Models.Player { Name = "ExistingPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var correctSequence = "A->C, A->B, C->B, A->C, B->A, B->C, A->C";
            var request = new TOHPSubmitRequest
            {
                NumPegs = 3,
                NumDisks = 3,
                UserMovesCount = 7,
                UserSequence = correctSequence,
                PlayerName = "ExistingPlayer"
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var players = _context.Players.Where(p => p.Name == "ExistingPlayer").ToList();
            Assert.Single(players);
        }

        [Fact]
        public void SubmitAnswer_Should_ReturnCorrectMovesFlag()
        {
            // Arrange
            var correctSequence = "A->C, A->B, C->B, A->C, B->A, B->C, A->C";
            var request = new TOHPSubmitRequest
            {
                NumPegs = 3,
                NumDisks = 3,
                UserMovesCount = 7,
                UserSequence = correctSequence,
                PlayerName = "TestPlayer"
            };

            // Act
            var result = _controller.SubmitAnswer(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void SubmitAnswer_Should_Handle4Pegs()
        {
            // Arrange
            var request = new TOHPSubmitRequest
            {
                NumPegs = 4,
                NumDisks = 5,
                UserMovesCount = 13,
                UserSequence = "A->B",
                PlayerName = "TestPlayer"
            };

            // Act
            var result = _controller.SubmitAnswer(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void SubmitAnswer_Should_HandleLargeNumberOfDisks()
        {
            // Arrange
            var request = new TOHPSubmitRequest
            {
                NumPegs = 3,
                NumDisks = 10,
                UserMovesCount = 1023,
                UserSequence = "A->C",
                PlayerName = "TestPlayer"
            };

            // Act
            var result = _controller.SubmitAnswer(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void SubmitAnswer_Should_SaveCorrectSequence()
        {
            // Arrange
            var correctSequence = "A->C, A->B, C->B, A->C, B->A, B->C, A->C";
            var request = new TOHPSubmitRequest
            {
                NumPegs = 3,
                NumDisks = 3,
                UserMovesCount = 7,
                UserSequence = correctSequence,
                PlayerName = "TestPlayer"
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var round = _context.HanoiRounds.FirstOrDefault();
            Assert.NotNull(round);
            Assert.NotEmpty(round.CorrectMoves_Sequence);
        }

        [Fact]
        public void SubmitAnswer_Should_SaveDatePlayed()
        {
            // Arrange
            var correctSequence = "A->C, A->B, C->B, A->C, B->A, B->C, A->C";
            var request = new TOHPSubmitRequest
            {
                NumPegs = 3,
                NumDisks = 3,
                UserMovesCount = 7,
                UserSequence = correctSequence,
                PlayerName = "TestPlayer"
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var round = _context.HanoiRounds.FirstOrDefault();
            Assert.NotNull(round);
            Assert.NotNull(round.DatePlayed);
        }
    }
}
