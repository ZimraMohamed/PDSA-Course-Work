using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDSA.API.Controllers.Games;
using PDSA.API.Data;
using PDSA.Core.Algorithms.TrafficSimulation;

namespace PDSA.Tests.Controllers
{
    public class TrafficControllerTests : IDisposable
    {
        private readonly PDSADbContext _context;
        private readonly TrafficController _controller;

        public TrafficControllerTests()
        {
            var options = new DbContextOptionsBuilder<PDSADbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new PDSADbContext(options);
            _controller = new TrafficController(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void SubmitAnswer_Should_ReturnBadRequest_ForEmptyPlayerName()
        {
            // Arrange
            var request = new TrafficRequest
            {
                PlayerName = "",
                PlayerAnswer = 10,
                Edges = new List<TrafficEdge>()
            };

            // Act
            var result = _controller.SubmitAnswer(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public void SubmitAnswer_Should_ReturnOkResult_ForValidRequest()
        {
            // Arrange
            var edges = new List<TrafficEdge>
            {
                new TrafficEdge { From = "A", To = "B", Capacity = 10 },
                new TrafficEdge { From = "A", To = "C", Capacity = 10 },
                new TrafficEdge { From = "B", To = "T", Capacity = 10 },
                new TrafficEdge { From = "C", To = "T", Capacity = 10 }
            };

            var request = new TrafficRequest
            {
                PlayerName = "TestPlayer",
                PlayerAnswer = 20,
                Edges = edges
            };

            // Act
            var result = _controller.SubmitAnswer(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void SubmitAnswer_Should_ReturnCorrectStatus_ForRightAnswer()
        {
            // Arrange
            var edges = new List<TrafficEdge>
            {
                new TrafficEdge { From = "A", To = "B", Capacity = 10 },
                new TrafficEdge { From = "A", To = "C", Capacity = 10 },
                new TrafficEdge { From = "B", To = "T", Capacity = 10 },
                new TrafficEdge { From = "C", To = "T", Capacity = 10 }
            };

            var request = new TrafficRequest
            {
                PlayerName = "TestPlayer",
                PlayerAnswer = 20,
                Edges = edges
            };

            // Act
            var result = _controller.SubmitAnswer(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);
        }

        [Fact]
        public void SubmitAnswer_Should_SaveToDatabase_ForCorrectAnswer()
        {
            // Arrange
            var edges = new List<TrafficEdge>
            {
                new TrafficEdge { From = "A", To = "B", Capacity = 10 },
                new TrafficEdge { From = "A", To = "C", Capacity = 10 },
                new TrafficEdge { From = "B", To = "T", Capacity = 10 },
                new TrafficEdge { From = "C", To = "T", Capacity = 10 }
            };

            var request = new TrafficRequest
            {
                PlayerName = "TestPlayer",
                PlayerAnswer = 20,
                Edges = edges
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var player = _context.Players.FirstOrDefault(p => p.Name == "TestPlayer");
            Assert.NotNull(player);
            
            var round = _context.TrafficRounds.FirstOrDefault(r => r.PlayerID == player.PlayerID);
            Assert.NotNull(round);
            Assert.Equal(20, round.CorrectMaxFlow);
        }

        [Fact]
        public void SubmitAnswer_Should_SaveAlgorithmTimes()
        {
            // Arrange
            var edges = new List<TrafficEdge>
            {
                new TrafficEdge { From = "A", To = "B", Capacity = 10 },
                new TrafficEdge { From = "A", To = "C", Capacity = 10 },
                new TrafficEdge { From = "B", To = "T", Capacity = 10 },
                new TrafficEdge { From = "C", To = "T", Capacity = 10 }
            };

            var request = new TrafficRequest
            {
                PlayerName = "TestPlayer",
                PlayerAnswer = 20,
                Edges = edges
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var algoTimes = _context.TrafficAlgoTimes.ToList();
            Assert.NotEmpty(algoTimes);
            Assert.Contains(algoTimes, at => at.AlgorithmName == "Edmonds-Karp");
            Assert.Contains(algoTimes, at => at.AlgorithmName == "Dinic");
        }

        [Fact]
        public void SubmitAnswer_Should_SaveEdgeCapacities()
        {
            // Arrange
            var edges = new List<TrafficEdge>
            {
                new TrafficEdge { From = "A", To = "B", Capacity = 10 },
                new TrafficEdge { From = "B", To = "T", Capacity = 10 },
                new TrafficEdge { From = "A", To = "T", Capacity = 15 }
            };

            var request = new TrafficRequest
            {
                PlayerName = "TestPlayer",
                PlayerAnswer = 25,
                Edges = edges
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var capacities = _context.TrafficCapacities.ToList();
            Assert.NotEmpty(capacities);
            Assert.Equal(3, capacities.Count);
        }

        [Fact]
        public void SubmitAnswer_Should_HandleSimpleNetwork()
        {
            // Arrange
            var edges = new List<TrafficEdge>
            {
                new TrafficEdge { From = "A", To = "T", Capacity = 10 }
            };

            var request = new TrafficRequest
            {
                PlayerName = "TestPlayer",
                PlayerAnswer = 10,
                Edges = edges
            };

            // Act
            var result = _controller.SubmitAnswer(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void SubmitAnswer_Should_HandleComplexNetwork()
        {
            // Arrange
            var edges = new List<TrafficEdge>
            {
                new TrafficEdge { From = "A", To = "B", Capacity = 10 },
                new TrafficEdge { From = "A", To = "C", Capacity = 10 },
                new TrafficEdge { From = "B", To = "C", Capacity = 2 },
                new TrafficEdge { From = "B", To = "T", Capacity = 4 },
                new TrafficEdge { From = "C", To = "T", Capacity = 9 }
            };

            var request = new TrafficRequest
            {
                PlayerName = "TestPlayer",
                PlayerAnswer = 13,
                Edges = edges
            };

            // Act
            var result = _controller.SubmitAnswer(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void SubmitAnswer_Should_CreatePlayer_IfNotExists()
        {
            // Arrange
            var edges = new List<TrafficEdge>
            {
                new TrafficEdge { From = "A", To = "T", Capacity = 10 }
            };

            var request = new TrafficRequest
            {
                PlayerName = "NewPlayer",
                PlayerAnswer = 10,
                Edges = edges
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

            var edges = new List<TrafficEdge>
            {
                new TrafficEdge { From = "A", To = "T", Capacity = 10 }
            };

            var request = new TrafficRequest
            {
                PlayerName = "ExistingPlayer",
                PlayerAnswer = 10,
                Edges = edges
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var players = _context.Players.Where(p => p.Name == "ExistingPlayer").ToList();
            Assert.Single(players);
        }

        [Fact]
        public void SubmitAnswer_Should_IncludeEdmondsKarpTime()
        {
            // Arrange
            var edges = new List<TrafficEdge>
            {
                new TrafficEdge { From = "A", To = "T", Capacity = 10 }
            };

            var request = new TrafficRequest
            {
                PlayerName = "TestPlayer",
                PlayerAnswer = 10,
                Edges = edges
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var edmondsKarpTime = _context.TrafficAlgoTimes.FirstOrDefault(at => at.AlgorithmName == "Edmonds-Karp");
            Assert.NotNull(edmondsKarpTime);
            Assert.True(edmondsKarpTime.TimeTaken_ms >= 0);
        }

        [Fact]
        public void SubmitAnswer_Should_IncludeDinicTime()
        {
            // Arrange
            var edges = new List<TrafficEdge>
            {
                new TrafficEdge { From = "A", To = "T", Capacity = 10 }
            };

            var request = new TrafficRequest
            {
                PlayerName = "TestPlayer",
                PlayerAnswer = 10,
                Edges = edges
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var dinicTime = _context.TrafficAlgoTimes.FirstOrDefault(at => at.AlgorithmName == "Dinic");
            Assert.NotNull(dinicTime);
            Assert.True(dinicTime.TimeTaken_ms >= 0);
        }

        [Fact]
        public void SubmitAnswer_Should_HandleEmptyEdgeList()
        {
            // Arrange
            var request = new TrafficRequest
            {
                PlayerName = "TestPlayer",
                PlayerAnswer = 0,
                Edges = new List<TrafficEdge>()
            };

            // Act
            var result = _controller.SubmitAnswer(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void SubmitAnswer_Should_SaveDatePlayed()
        {
            // Arrange
            var edges = new List<TrafficEdge>
            {
                new TrafficEdge { From = "A", To = "T", Capacity = 10 }
            };

            var request = new TrafficRequest
            {
                PlayerName = "TestPlayer",
                PlayerAnswer = 10,
                Edges = edges
            };

            // Act
            _controller.SubmitAnswer(request);

            // Assert
            var round = _context.TrafficRounds.FirstOrDefault();
            Assert.NotNull(round);
            Assert.NotNull(round.DatePlayed);
        }
    }
}
