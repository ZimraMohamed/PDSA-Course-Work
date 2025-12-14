using PDSA.API.Data.Models;
using PDSA.API.Data.Models.EightQueens;
using PDSA.API.Data.Models.SnakeAndLadder;
using PDSA.API.Data.Models.TowerOfHanoi;
using PDSA.API.Data.Models.TrafficSimulation;
using PDSA.API.Data.Models.TSP;
using System.ComponentModel.DataAnnotations;

namespace PDSA.Tests.Data.Models
{
    public class PlayerTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void Player_Should_BeInstantiated()
        {
            // Act
            var player = new Player();

            // Assert
            Assert.NotNull(player);
        }

        [Fact]
        public void Player_Should_HaveDefaultValues()
        {
            // Act
            var player = new Player();

            // Assert
            Assert.Equal(0, player.PlayerID);
            Assert.Equal(string.Empty, player.Name);
            Assert.NotNull(player.TSPRounds);
            Assert.Empty(player.TSPRounds);
            Assert.NotNull(player.EQPSolutions);
            Assert.Empty(player.EQPSolutions);
            Assert.NotNull(player.TrafficRounds);
            Assert.Empty(player.TrafficRounds);
            Assert.NotNull(player.HanoiRounds);
            Assert.Empty(player.HanoiRounds);
            Assert.NotNull(player.SnakeLadderRounds);
            Assert.Empty(player.SnakeLadderRounds);
        }

        [Fact]
        public void Player_Should_SetPlayerID()
        {
            // Arrange
            var player = new Player();

            // Act
            player.PlayerID = 42;

            // Assert
            Assert.Equal(42, player.PlayerID);
        }

        [Fact]
        public void Player_Should_SetName()
        {
            // Arrange
            var player = new Player();

            // Act
            player.Name = "John Doe";

            // Assert
            Assert.Equal("John Doe", player.Name);
        }

        [Fact]
        public void Player_Should_AllowObjectInitializer()
        {
            // Act
            var player = new Player
            {
                PlayerID = 123,
                Name = "Jane Smith"
            };

            // Assert
            Assert.Equal(123, player.PlayerID);
            Assert.Equal("Jane Smith", player.Name);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void Player_Name_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(Player).GetProperty(nameof(Player.Name));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
            Assert.IsType<RequiredAttribute>(attribute);
        }

        [Fact]
        public void Player_PlayerID_Should_HaveKeyAttribute()
        {
            // Arrange
            var property = typeof(Player).GetProperty(nameof(Player.PlayerID));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
            Assert.IsType<KeyAttribute>(attribute);
        }

        [Fact]
        public void Player_Should_FailValidation_WhenNameIsEmpty()
        {
            // Arrange
            var player = new Player { Name = "" };
            var validationContext = new ValidationContext(player);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(player, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(Player.Name)));
        }

        [Fact]
        public void Player_Should_PassValidation_WhenNameIsProvided()
        {
            // Arrange
            var player = new Player { Name = "Valid Name" };
            var validationContext = new ValidationContext(player);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(player, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        #endregion

        #region Navigation Property Tests

        [Fact]
        public void Player_Should_AddTSPRound()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            var tspRound = new TSPRound
            {
                PlayerID = player.PlayerID,
                HomeCity = "Delhi",
                SelectedCities = "Mumbai",
                ShortestRoute_Path = "Delhi->Mumbai",
                ShortestRoute_Distance = 1400,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            // Act
            player.TSPRounds.Add(tspRound);

            // Assert
            Assert.Single(player.TSPRounds);
            Assert.Contains(tspRound, player.TSPRounds);
        }

        [Fact]
        public void Player_Should_AddEQPSolution()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer", PlayerID = 1 };
            var solution = new EQPSolution
            {
                PlayerID = player.PlayerID,
                Solution_Text = "0-0,1-4,2-7,3-5,4-2,5-6,6-1,7-3",
                IsFound = true,
                DateFound = DateTime.UtcNow.ToString("o")
            };

            // Act
            player.EQPSolutions.Add(solution);

            // Assert
            Assert.Single(player.EQPSolutions);
            Assert.Contains(solution, player.EQPSolutions);
        }

        [Fact]
        public void Player_Should_AddTrafficRound()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            var trafficRound = new TrafficRound
            {
                PlayerID = player.PlayerID,
                CorrectMaxFlow = 20,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            // Act
            player.TrafficRounds.Add(trafficRound);

            // Assert
            Assert.Single(player.TrafficRounds);
            Assert.Contains(trafficRound, player.TrafficRounds);
        }

        [Fact]
        public void Player_Should_AddHanoiRound()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            var hanoiRound = new HanoiRound
            {
                PlayerID = player.PlayerID,
                NumDisks_N = 3,
                NumPegs = 3,
                CorrectMoves_Count = 7,
                CorrectMoves_Sequence = "A->C",
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            // Act
            player.HanoiRounds.Add(hanoiRound);

            // Assert
            Assert.Single(player.HanoiRounds);
            Assert.Contains(hanoiRound, player.HanoiRounds);
        }

        [Fact]
        public void Player_Should_AddSnakeLadderRound()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            var snakeLadderRound = new SnakeLadderRound
            {
                PlayerID = player.PlayerID,
                BoardSize_N = 100,
                NumLadders = 5,
                NumSnakes = 5,
                CorrectMinThrows = 7,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            // Act
            player.SnakeLadderRounds.Add(snakeLadderRound);

            // Assert
            Assert.Single(player.SnakeLadderRounds);
            Assert.Contains(snakeLadderRound, player.SnakeLadderRounds);
        }

        [Fact]
        public void Player_Should_AddMultipleTSPRounds()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            var round1 = new TSPRound
            {
                HomeCity = "Delhi",
                SelectedCities = "Mumbai",
                ShortestRoute_Path = "Delhi->Mumbai",
                ShortestRoute_Distance = 1400,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            var round2 = new TSPRound
            {
                HomeCity = "Kolkata",
                SelectedCities = "Chennai",
                ShortestRoute_Path = "Kolkata->Chennai",
                ShortestRoute_Distance = 1650,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            // Act
            player.TSPRounds.Add(round1);
            player.TSPRounds.Add(round2);

            // Assert
            Assert.Equal(2, player.TSPRounds.Count);
            Assert.Contains(round1, player.TSPRounds);
            Assert.Contains(round2, player.TSPRounds);
        }

        [Fact]
        public void Player_Should_SupportAllGameTypes()
        {
            // Arrange
            var player = new Player { Name = "MultiGamePlayer", PlayerID = 1 };

            var tspRound = new TSPRound
            {
                HomeCity = "Delhi",
                SelectedCities = "Mumbai",
                ShortestRoute_Path = "Delhi->Mumbai",
                ShortestRoute_Distance = 1400,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            var eqpSolution = new EQPSolution
            {
                Solution_Text = "0-0,1-4,2-7,3-5,4-2,5-6,6-1,7-3",
                IsFound = true,
                DateFound = DateTime.UtcNow.ToString("o")
            };

            var trafficRound = new TrafficRound
            {
                CorrectMaxFlow = 20,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            var hanoiRound = new HanoiRound
            {
                NumDisks_N = 3,
                NumPegs = 3,
                CorrectMoves_Count = 7,
                CorrectMoves_Sequence = "A->C",
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            var snakeLadderRound = new SnakeLadderRound
            {
                BoardSize_N = 100,
                NumLadders = 5,
                NumSnakes = 5,
                CorrectMinThrows = 7,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            // Act
            player.TSPRounds.Add(tspRound);
            player.EQPSolutions.Add(eqpSolution);
            player.TrafficRounds.Add(trafficRound);
            player.HanoiRounds.Add(hanoiRound);
            player.SnakeLadderRounds.Add(snakeLadderRound);

            // Assert
            Assert.Single(player.TSPRounds);
            Assert.Single(player.EQPSolutions);
            Assert.Single(player.TrafficRounds);
            Assert.Single(player.HanoiRounds);
            Assert.Single(player.SnakeLadderRounds);
        }

        [Fact]
        public void Player_Should_RemoveTSPRound()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            var round = new TSPRound
            {
                HomeCity = "Delhi",
                SelectedCities = "Mumbai",
                ShortestRoute_Path = "Delhi->Mumbai",
                ShortestRoute_Distance = 1400,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            player.TSPRounds.Add(round);

            // Act
            player.TSPRounds.Remove(round);

            // Assert
            Assert.Empty(player.TSPRounds);
        }

        [Fact]
        public void Player_Should_ClearAllRounds()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            player.TSPRounds.Add(new TSPRound
            {
                HomeCity = "Delhi",
                SelectedCities = "Mumbai",
                ShortestRoute_Path = "Delhi->Mumbai",
                ShortestRoute_Distance = 1400,
                DatePlayed = DateTime.UtcNow.ToString("o")
            });
            player.TrafficRounds.Add(new TrafficRound
            {
                CorrectMaxFlow = 20,
                DatePlayed = DateTime.UtcNow.ToString("o")
            });

            // Act
            player.TSPRounds.Clear();
            player.TrafficRounds.Clear();

            // Assert
            Assert.Empty(player.TSPRounds);
            Assert.Empty(player.TrafficRounds);
        }

        #endregion

        #region Equality and Reference Tests

        [Fact]
        public void Player_Should_BeDifferentInstances()
        {
            // Arrange
            var player1 = new Player { Name = "Player1" };
            var player2 = new Player { Name = "Player1" };

            // Assert
            Assert.NotSame(player1, player2);
        }

        [Fact]
        public void Player_Should_MaintainSameReference()
        {
            // Arrange
            var player1 = new Player { Name = "Player1" };
            var player2 = player1;

            // Act
            player2.Name = "UpdatedName";

            // Assert
            Assert.Same(player1, player2);
            Assert.Equal("UpdatedName", player1.Name);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void Player_Should_AllowVeryLongName()
        {
            // Arrange
            var longName = new string('A', 1000);
            var player = new Player();

            // Act
            player.Name = longName;

            // Assert
            Assert.Equal(longName, player.Name);
            Assert.Equal(1000, player.Name.Length);
        }

        [Fact]
        public void Player_Should_AllowSpecialCharactersInName()
        {
            // Arrange
            var player = new Player();

            // Act
            player.Name = "João O'Brien-Smith (Jr.) #1";

            // Assert
            Assert.Equal("João O'Brien-Smith (Jr.) #1", player.Name);
        }

        [Fact]
        public void Player_Should_AllowUnicodeCharactersInName()
        {
            // Arrange
            var player = new Player();

            // Act
            player.Name = "测试玩家 😀";

            // Assert
            Assert.Equal("测试玩家 😀", player.Name);
        }

        [Fact]
        public void Player_NavigationProperties_Should_BeModifiable()
        {
            // Arrange
            var player = new Player { Name = "TestPlayer" };
            var newCollection = new List<TSPRound>();

            // Act
            player.TSPRounds = newCollection;

            // Assert
            Assert.Same(newCollection, player.TSPRounds);
        }

        #endregion
    }
}
