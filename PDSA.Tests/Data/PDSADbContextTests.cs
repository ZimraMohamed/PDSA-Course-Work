using Microsoft.EntityFrameworkCore;
using PDSA.API.Data;
using PDSA.API.Data.Models;
using PDSA.API.Data.Models.EightQueens;
using PDSA.API.Data.Models.SnakeAndLadder;
using PDSA.API.Data.Models.TowerOfHanoi;
using PDSA.API.Data.Models.TrafficSimulation;
using PDSA.API.Data.Models.TSP;

namespace PDSA.Tests.Data
{
    public class PDSADbContextTests : IDisposable
    {
        private readonly PDSADbContext _context;

        public PDSADbContextTests()
        {
            var options = new DbContextOptionsBuilder<PDSADbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new PDSADbContext(options);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region DbContext Creation Tests

        [Fact]
        public void DbContext_Should_BeCreated()
        {
            // Assert
            Assert.NotNull(_context);
        }

        [Fact]
        public void DbContext_Should_HaveAllDbSets()
        {
            // Assert
            Assert.NotNull(_context.Players);
            Assert.NotNull(_context.TSPRounds);
            Assert.NotNull(_context.TSPDistances);
            Assert.NotNull(_context.TSPAlgoTimes);
            Assert.NotNull(_context.EQPSolutions);
            Assert.NotNull(_context.EQPAlgoTimes);
            Assert.NotNull(_context.TrafficRounds);
            Assert.NotNull(_context.TrafficCapacities);
            Assert.NotNull(_context.TrafficAlgoTimes);
            Assert.NotNull(_context.HanoiRounds);
            Assert.NotNull(_context.HanoiAlgoTimes);
            Assert.NotNull(_context.SnakeLadderRounds);
            Assert.NotNull(_context.SnakeLadderBoardConfigs);
            Assert.NotNull(_context.SnakeLadderAlgoTimes);
        }

        #endregion

        #region Player Entity Tests

        [Fact]
        public void Player_Should_BeAddedToDatabase()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };

            // Act
            _context.Players.Add(player);
            _context.SaveChanges();

            // Assert
            var savedPlayer = _context.Players.FirstOrDefault(p => p.Name == "TestPlayer");
            Assert.NotNull(savedPlayer);
            Assert.True(savedPlayer.PlayerID > 0);
        }

        [Fact]
        public void Player_Should_GenerateIdOnAdd()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };

            // Act
            _context.Players.Add(player);
            _context.SaveChanges();

            // Assert
            Assert.True(player.PlayerID > 0);
        }

        [Fact]
        public void Player_Should_RequireName()
        {
            // Arrange
            var player = new Player { Name = null! };

            // Act & Assert
            _context.Players.Add(player);
            Assert.Throws<DbUpdateException>(() => _context.SaveChanges());
        }

        #endregion

        #region TSP Entity Tests

        [Fact]
        public void TSPRound_Should_BeAddedToDatabase()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new TSPRound
            {
                PlayerID = player.PlayerID,
                HomeCity = "Delhi",
                SelectedCities = "Mumbai,Kolkata",
                ShortestRoute_Path = "Delhi->Mumbai->Kolkata->Delhi",
                ShortestRoute_Distance = 2500,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            // Act
            _context.TSPRounds.Add(round);
            _context.SaveChanges();

            // Assert
            var savedRound = _context.TSPRounds.FirstOrDefault(r => r.HomeCity == "Delhi");
            Assert.NotNull(savedRound);
            Assert.True(savedRound.RoundID > 0);
        }

        [Fact]
        public void TSPRound_Should_HaveForeignKeyToPlayer()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new TSPRound
            {
                PlayerID = player.PlayerID,
                HomeCity = "Delhi",
                SelectedCities = "Mumbai",
                ShortestRoute_Path = "Delhi->Mumbai->Delhi",
                ShortestRoute_Distance = 1400,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            // Act
            _context.TSPRounds.Add(round);
            _context.SaveChanges();

            // Assert
            var savedRound = _context.TSPRounds.Include(r => r.Player).FirstOrDefault();
            Assert.NotNull(savedRound);
            Assert.NotNull(savedRound.Player);
            Assert.Equal("TestPlayer", savedRound.Player.Name);
        }

        [Fact]
        public void TSPRound_Should_CascadeDeleteWithPlayer()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new TSPRound
            {
                PlayerID = player.PlayerID,
                HomeCity = "Delhi",
                SelectedCities = "Mumbai",
                ShortestRoute_Path = "Delhi->Mumbai->Delhi",
                ShortestRoute_Distance = 1400,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            _context.TSPRounds.Add(round);
            _context.SaveChanges();

            // Act
            _context.Players.Remove(player);
            _context.SaveChanges();

            // Assert
            var rounds = _context.TSPRounds.ToList();
            Assert.Empty(rounds);
        }

        [Fact]
        public void TSPDistance_Should_BeAddedToDatabase()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new TSPRound
            {
                PlayerID = player.PlayerID,
                HomeCity = "Delhi",
                SelectedCities = "Mumbai",
                ShortestRoute_Path = "Delhi->Mumbai->Delhi",
                ShortestRoute_Distance = 1400,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            _context.TSPRounds.Add(round);
            _context.SaveChanges();

            var distance = new TSPDistance
            {
                RoundID = round.RoundID,
                City_A = "Delhi",
                City_B = "Mumbai",
                Distance_km = 1400
            };

            // Act
            _context.TSPDistances.Add(distance);
            _context.SaveChanges();

            // Assert
            var savedDistance = _context.TSPDistances.FirstOrDefault();
            Assert.NotNull(savedDistance);
            Assert.Equal("Delhi", savedDistance.City_A);
            Assert.Equal("Mumbai", savedDistance.City_B);
        }

        [Fact]
        public void TSPAlgoTime_Should_BeAddedToDatabase()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new TSPRound
            {
                PlayerID = player.PlayerID,
                HomeCity = "Delhi",
                SelectedCities = "Mumbai",
                ShortestRoute_Path = "Delhi->Mumbai->Delhi",
                ShortestRoute_Distance = 1400,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            _context.TSPRounds.Add(round);
            _context.SaveChanges();

            var algoTime = new TSPAlgoTime
            {
                RoundID = round.RoundID,
                AlgorithmName = "Branch and Bound",
                TimeTaken_ms = 50.5
            };

            // Act
            _context.TSPAlgoTimes.Add(algoTime);
            _context.SaveChanges();

            // Assert
            var savedTime = _context.TSPAlgoTimes.FirstOrDefault();
            Assert.NotNull(savedTime);
            Assert.Equal("Branch and Bound", savedTime.AlgorithmName);
        }

        #endregion

        #region EightQueens Entity Tests

        [Fact]
        public void EQPSolution_Should_BeAddedToDatabase()
        {
            // Arrange
            var solution = new EQPSolution
            {
                Solution_Text = "0-0,1-4,2-7,3-5,4-2,5-6,6-1,7-3",
                IsFound = false
            };

            // Act
            _context.EQPSolutions.Add(solution);
            _context.SaveChanges();

            // Assert
            var savedSolution = _context.EQPSolutions.FirstOrDefault();
            Assert.NotNull(savedSolution);
            Assert.True(savedSolution.SolutionID > 0);
            Assert.False(savedSolution.IsFound);
        }

        [Fact]
        public void EQPSolution_Should_AllowDifferentSolutionTexts()
        {
            // Arrange
            var solution1 = new EQPSolution
            {
                Solution_Text = "0-0,1-4,2-7,3-5,4-2,5-6,6-1,7-3",
                IsFound = false
            };
            var solution2 = new EQPSolution
            {
                Solution_Text = "0-1,1-3,2-5,3-7,4-2,5-0,6-6,7-4",
                IsFound = false
            };

            // Act
            _context.EQPSolutions.Add(solution1);
            _context.EQPSolutions.Add(solution2);
            _context.SaveChanges();

            // Assert
            var solutions = _context.EQPSolutions.ToList();
            Assert.Equal(2, solutions.Count);
        }

        [Fact]
        public void EQPSolution_Should_LinkToPlayer()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var solution = new EQPSolution
            {
                Solution_Text = "0-0,1-4,2-7,3-5,4-2,5-6,6-1,7-3",
                IsFound = true,
                PlayerID = player.PlayerID,
                DateFound = DateTime.UtcNow.ToString("o")
            };

            // Act
            _context.EQPSolutions.Add(solution);
            _context.SaveChanges();

            // Assert
            var savedSolution = _context.EQPSolutions.Include(s => s.Player).FirstOrDefault();
            Assert.NotNull(savedSolution);
            Assert.NotNull(savedSolution.Player);
            Assert.Equal("TestPlayer", savedSolution.Player.Name);
        }

        [Fact]
        public void EQPAlgoTime_Should_BeAddedToDatabase()
        {
            // Arrange
            var algoTime = new EQPAlgoTime
            {
                DateExecuted = DateTime.UtcNow.ToString("o"),
                AlgorithmType = "Sequential Backtracking",
                TimeTaken_ms = 125.5,
                RoundNumber = 1
            };

            // Act
            _context.EQPAlgoTimes.Add(algoTime);
            _context.SaveChanges();

            // Assert
            var savedTime = _context.EQPAlgoTimes.FirstOrDefault();
            Assert.NotNull(savedTime);
            Assert.Equal("Sequential Backtracking", savedTime.AlgorithmType);
        }

        #endregion

        #region Traffic Simulation Entity Tests

        [Fact]
        public void TrafficRound_Should_BeAddedToDatabase()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new TrafficRound
            {
                PlayerID = player.PlayerID,
                CorrectMaxFlow = 20,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            // Act
            _context.TrafficRounds.Add(round);
            _context.SaveChanges();

            // Assert
            var savedRound = _context.TrafficRounds.FirstOrDefault();
            Assert.NotNull(savedRound);
            Assert.Equal(20, savedRound.CorrectMaxFlow);
        }

        [Fact]
        public void TrafficCapacity_Should_BeAddedToDatabase()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new TrafficRound
            {
                PlayerID = player.PlayerID,
                CorrectMaxFlow = 20,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            _context.TrafficRounds.Add(round);
            _context.SaveChanges();

            var capacity = new TrafficCapacity
            {
                RoundID = round.RoundID,
                RoadSegment = "A-B",
                Capacity_VehPerMin = 10
            };

            // Act
            _context.TrafficCapacities.Add(capacity);
            _context.SaveChanges();

            // Assert
            var savedCapacity = _context.TrafficCapacities.FirstOrDefault();
            Assert.NotNull(savedCapacity);
            Assert.Equal("A-B", savedCapacity.RoadSegment);
        }

        [Fact]
        public void TrafficAlgoTime_Should_BeAddedToDatabase()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new TrafficRound
            {
                PlayerID = player.PlayerID,
                CorrectMaxFlow = 20,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            _context.TrafficRounds.Add(round);
            _context.SaveChanges();

            var algoTime = new TrafficAlgoTime
            {
                RoundID = round.RoundID,
                AlgorithmName = "Edmonds-Karp",
                TimeTaken_ms = 75.2
            };

            // Act
            _context.TrafficAlgoTimes.Add(algoTime);
            _context.SaveChanges();

            // Assert
            var savedTime = _context.TrafficAlgoTimes.FirstOrDefault();
            Assert.NotNull(savedTime);
            Assert.Equal("Edmonds-Karp", savedTime.AlgorithmName);
        }

        [Fact]
        public void TrafficRound_Should_CascadeDeleteChildren()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new TrafficRound
            {
                PlayerID = player.PlayerID,
                CorrectMaxFlow = 20,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            _context.TrafficRounds.Add(round);
            _context.SaveChanges();

            var capacity = new TrafficCapacity
            {
                RoundID = round.RoundID,
                RoadSegment = "A-B",
                Capacity_VehPerMin = 10
            };
            _context.TrafficCapacities.Add(capacity);
            _context.SaveChanges();

            // Act
            _context.TrafficRounds.Remove(round);
            _context.SaveChanges();

            // Assert
            var capacities = _context.TrafficCapacities.ToList();
            Assert.Empty(capacities);
        }

        #endregion

        #region Tower of Hanoi Entity Tests

        [Fact]
        public void HanoiRound_Should_BeAddedToDatabase()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new HanoiRound
            {
                PlayerID = player.PlayerID,
                NumDisks_N = 3,
                NumPegs = 3,
                CorrectMoves_Count = 7,
                CorrectMoves_Sequence = "A->C,A->B,C->B,A->C,B->A,B->C,A->C",
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            // Act
            _context.HanoiRounds.Add(round);
            _context.SaveChanges();

            // Assert
            var savedRound = _context.HanoiRounds.FirstOrDefault();
            Assert.NotNull(savedRound);
            Assert.Equal(3, savedRound.NumDisks_N);
            Assert.Equal(7, savedRound.CorrectMoves_Count);
        }

        [Fact]
        public void HanoiAlgoTime_Should_BeAddedToDatabase()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new HanoiRound
            {
                PlayerID = player.PlayerID,
                NumDisks_N = 3,
                NumPegs = 3,
                CorrectMoves_Count = 7,
                CorrectMoves_Sequence = "A->C,A->B,C->B,A->C,B->A,B->C,A->C",
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            _context.HanoiRounds.Add(round);
            _context.SaveChanges();

            var algoTime = new HanoiAlgoTime
            {
                RoundID = round.RoundID,
                AlgorithmName = "Recursive",
                TimeTaken_ms = 15.3
            };

            // Act
            _context.HanoiAlgoTimes.Add(algoTime);
            _context.SaveChanges();

            // Assert
            var savedTime = _context.HanoiAlgoTimes.FirstOrDefault();
            Assert.NotNull(savedTime);
            Assert.Equal("Recursive", savedTime.AlgorithmName);
        }

        #endregion

        #region Snake and Ladder Entity Tests

        [Fact]
        public void SnakeLadderRound_Should_BeAddedToDatabase()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new SnakeLadderRound
            {
                PlayerID = player.PlayerID,
                BoardSize_N = 100,
                NumLadders = 5,
                NumSnakes = 5,
                CorrectMinThrows = 7,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            // Act
            _context.SnakeLadderRounds.Add(round);
            _context.SaveChanges();

            // Assert
            var savedRound = _context.SnakeLadderRounds.FirstOrDefault();
            Assert.NotNull(savedRound);
            Assert.Equal(100, savedRound.BoardSize_N);
            Assert.Equal(7, savedRound.CorrectMinThrows);
        }

        [Fact]
        public void SnakeLadderBoardConfig_Should_BeAddedToDatabase()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new SnakeLadderRound
            {
                PlayerID = player.PlayerID,
                BoardSize_N = 100,
                NumLadders = 1,
                NumSnakes = 1,
                CorrectMinThrows = 7,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            _context.SnakeLadderRounds.Add(round);
            _context.SaveChanges();

            var config = new SnakeLadderBoardConfig
            {
                RoundID = round.RoundID,
                FeatureType = "Ladder",
                Start_Cell = 3,
                End_Cell = 22
            };

            // Act
            _context.SnakeLadderBoardConfigs.Add(config);
            _context.SaveChanges();

            // Assert
            var savedConfig = _context.SnakeLadderBoardConfigs.FirstOrDefault();
            Assert.NotNull(savedConfig);
            Assert.Equal("Ladder", savedConfig.FeatureType);
            Assert.Equal(3, savedConfig.Start_Cell);
        }

        [Fact]
        public void SnakeLadderAlgoTime_Should_BeAddedToDatabase()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new SnakeLadderRound
            {
                PlayerID = player.PlayerID,
                BoardSize_N = 100,
                NumLadders = 5,
                NumSnakes = 5,
                CorrectMinThrows = 7,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            _context.SnakeLadderRounds.Add(round);
            _context.SaveChanges();

            var algoTime = new SnakeLadderAlgoTime
            {
                RoundID = round.RoundID,
                AlgorithmName = "BFS",
                TimeTaken_ms = 42.7
            };

            // Act
            _context.SnakeLadderAlgoTimes.Add(algoTime);
            _context.SaveChanges();

            // Assert
            var savedTime = _context.SnakeLadderAlgoTimes.FirstOrDefault();
            Assert.NotNull(savedTime);
            Assert.Equal("BFS", savedTime.AlgorithmName);
        }

        [Fact]
        public void SnakeLadderRound_Should_CascadeDeleteChildren()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var round = new SnakeLadderRound
            {
                PlayerID = player.PlayerID,
                BoardSize_N = 100,
                NumLadders = 1,
                NumSnakes = 1,
                CorrectMinThrows = 7,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            _context.SnakeLadderRounds.Add(round);
            _context.SaveChanges();

            var config = new SnakeLadderBoardConfig
            {
                RoundID = round.RoundID,
                FeatureType = "Ladder",
                Start_Cell = 3,
                End_Cell = 22
            };
            _context.SnakeLadderBoardConfigs.Add(config);
            _context.SaveChanges();

            // Act
            _context.SnakeLadderRounds.Remove(round);
            _context.SaveChanges();

            // Assert
            var configs = _context.SnakeLadderBoardConfigs.ToList();
            Assert.Empty(configs);
        }

        #endregion

        #region Complex Relationship Tests

        [Fact]
        public void Player_Should_SupportMultipleGameRounds()
        {
            // Arrange
            var player = new Player { Name = "MultiGamePlayer" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var tspRound = new TSPRound
            {
                PlayerID = player.PlayerID,
                HomeCity = "Delhi",
                SelectedCities = "Mumbai",
                ShortestRoute_Path = "Delhi->Mumbai->Delhi",
                ShortestRoute_Distance = 1400,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            var trafficRound = new TrafficRound
            {
                PlayerID = player.PlayerID,
                CorrectMaxFlow = 20,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            var hanoiRound = new HanoiRound
            {
                PlayerID = player.PlayerID,
                NumDisks_N = 3,
                NumPegs = 3,
                CorrectMoves_Count = 7,
                CorrectMoves_Sequence = "A->C",
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            _context.TSPRounds.Add(tspRound);
            _context.TrafficRounds.Add(trafficRound);
            _context.HanoiRounds.Add(hanoiRound);
            _context.SaveChanges();

            // Act
            var savedPlayer = _context.Players
                .Include(p => p.TSPRounds)
                .Include(p => p.TrafficRounds)
                .Include(p => p.HanoiRounds)
                .FirstOrDefault(p => p.Name == "MultiGamePlayer");

            // Assert
            Assert.NotNull(savedPlayer);
            Assert.Single(savedPlayer.TSPRounds);
            Assert.Single(savedPlayer.TrafficRounds);
            Assert.Single(savedPlayer.HanoiRounds);
        }

        [Fact]
        public void Player_Deletion_Should_CascadeToAllGameRounds()
        {
            // Arrange
            var player = new Player { Name = "ToBeDeleted" };
            _context.Players.Add(player);
            _context.SaveChanges();

            var tspRound = new TSPRound
            {
                PlayerID = player.PlayerID,
                HomeCity = "Delhi",
                SelectedCities = "Mumbai",
                ShortestRoute_Path = "Delhi->Mumbai->Delhi",
                ShortestRoute_Distance = 1400,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            var trafficRound = new TrafficRound
            {
                PlayerID = player.PlayerID,
                CorrectMaxFlow = 20,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            _context.TSPRounds.Add(tspRound);
            _context.TrafficRounds.Add(trafficRound);
            _context.SaveChanges();

            // Act
            _context.Players.Remove(player);
            _context.SaveChanges();

            // Assert
            Assert.Empty(_context.TSPRounds.ToList());
            Assert.Empty(_context.TrafficRounds.ToList());
        }

        [Fact]
        public void DbContext_Should_SupportConcurrentOperations()
        {
            // Arrange
            var player1 = new Player { Name = "Player1" };
            var player2 = new Player { Name = "Player2" };

            // Act
            _context.Players.AddRange(player1, player2);
            _context.SaveChanges();

            // Assert
            var players = _context.Players.ToList();
            Assert.Equal(2, players.Count);
            Assert.Contains(players, p => p.Name == "Player1");
            Assert.Contains(players, p => p.Name == "Player2");
        }

        #endregion
    }
}
