using System.ComponentModel.DataAnnotations;
using PDSA.API.Data.Models;
using PDSA.API.Data.Models.SnakeAndLadder;

namespace PDSA.Tests.Data.Models.SnakeAndLadder
{
    public class SnakeLadderRoundTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var round = new SnakeLadderRound();

            // Assert
            Assert.Equal(0, round.RoundID);
            Assert.Equal(0, round.PlayerID);
            Assert.Equal(0, round.BoardSize_N);
            Assert.Equal(0, round.NumLadders);
            Assert.Equal(0, round.NumSnakes);
            Assert.Equal(0, round.CorrectMinThrows);
            Assert.NotEmpty(round.DatePlayed); // Has default DateTime value
            Assert.Null(round.Player);
            Assert.NotNull(round.BoardConfigs);
            Assert.Empty(round.BoardConfigs);
            Assert.NotNull(round.AlgorithmTimes);
            Assert.Empty(round.AlgorithmTimes);
        }

        [Fact]
        public void Properties_ShouldSetAndGetValues()
        {
            // Arrange
            var round = new SnakeLadderRound();
            var dateTime = "2024-01-15 10:30:00";

            // Act
            round.RoundID = 123;
            round.PlayerID = 456;
            round.BoardSize_N = 100;
            round.NumLadders = 10;
            round.NumSnakes = 8;
            round.CorrectMinThrows = 15;
            round.DatePlayed = dateTime;

            // Assert
            Assert.Equal(123, round.RoundID);
            Assert.Equal(456, round.PlayerID);
            Assert.Equal(100, round.BoardSize_N);
            Assert.Equal(10, round.NumLadders);
            Assert.Equal(8, round.NumSnakes);
            Assert.Equal(15, round.CorrectMinThrows);
            Assert.Equal(dateTime, round.DatePlayed);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void RoundID_ShouldHaveKeyAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderRound).GetProperty(nameof(SnakeLadderRound.RoundID));

            // Act
            var keyAttribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(keyAttribute);
        }

        [Fact]
        public void PlayerID_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderRound).GetProperty(nameof(SnakeLadderRound.PlayerID));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void BoardSize_N_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderRound).GetProperty(nameof(SnakeLadderRound.BoardSize_N));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void NumLadders_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderRound).GetProperty(nameof(SnakeLadderRound.NumLadders));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void NumSnakes_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderRound).GetProperty(nameof(SnakeLadderRound.NumSnakes));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void CorrectMinThrows_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderRound).GetProperty(nameof(SnakeLadderRound.CorrectMinThrows));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void DatePlayed_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderRound).GetProperty(nameof(SnakeLadderRound.DatePlayed));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        #endregion

        #region Navigation Property Tests

        [Fact]
        public void Player_ShouldSetAndGetValue()
        {
            // Arrange
            var round = new SnakeLadderRound();
            var player = new Player
            {
                PlayerID = 1,
                Name = "TestPlayer"
            };

            // Act
            round.Player = player;

            // Assert
            Assert.NotNull(round.Player);
            Assert.Equal(1, round.Player.PlayerID);
            Assert.Equal("TestPlayer", round.Player.Name);
        }

        [Fact]
        public void BoardConfigs_ShouldAddMultipleItems()
        {
            // Arrange
            var round = new SnakeLadderRound { RoundID = 1 };
            var ladder = new SnakeLadderBoardConfig
            {
                ConfigID = 1,
                RoundID = 1,
                FeatureType = "Ladder",
                Start_Cell = 3,
                End_Cell = 12
            };
            var snake = new SnakeLadderBoardConfig
            {
                ConfigID = 2,
                RoundID = 1,
                FeatureType = "Snake",
                Start_Cell = 15,
                End_Cell = 6
            };

            // Act
            round.BoardConfigs.Add(ladder);
            round.BoardConfigs.Add(snake);

            // Assert
            Assert.Equal(2, round.BoardConfigs.Count);
            Assert.Contains(ladder, round.BoardConfigs);
            Assert.Contains(snake, round.BoardConfigs);
        }

        [Fact]
        public void AlgorithmTimes_ShouldAddMultipleItems()
        {
            // Arrange
            var round = new SnakeLadderRound { RoundID = 1 };
            var time1 = new SnakeLadderAlgoTime
            {
                TimeID = 1,
                RoundID = 1,
                AlgorithmName = "BFS",
                TimeTaken_ms = 2.5
            };
            var time2 = new SnakeLadderAlgoTime
            {
                TimeID = 2,
                RoundID = 1,
                AlgorithmName = "Dijkstra",
                TimeTaken_ms = 3.2
            };

            // Act
            round.AlgorithmTimes.Add(time1);
            round.AlgorithmTimes.Add(time2);

            // Assert
            Assert.Equal(2, round.AlgorithmTimes.Count);
            Assert.Contains(time1, round.AlgorithmTimes);
            Assert.Contains(time2, round.AlgorithmTimes);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void BoardSize_N_ShouldHandleSmallBoard()
        {
            // Arrange
            var round = new SnakeLadderRound();

            // Act
            round.BoardSize_N = 10;

            // Assert
            Assert.Equal(10, round.BoardSize_N);
        }

        [Fact]
        public void BoardSize_N_ShouldHandleLargeBoard()
        {
            // Arrange
            var round = new SnakeLadderRound();

            // Act
            round.BoardSize_N = 1000;

            // Assert
            Assert.Equal(1000, round.BoardSize_N);
        }

        [Fact]
        public void NumLadders_ShouldHandleZero()
        {
            // Arrange
            var round = new SnakeLadderRound();

            // Act
            round.NumLadders = 0;

            // Assert
            Assert.Equal(0, round.NumLadders);
        }

        [Fact]
        public void NumSnakes_ShouldHandleZero()
        {
            // Arrange
            var round = new SnakeLadderRound();

            // Act
            round.NumSnakes = 0;

            // Assert
            Assert.Equal(0, round.NumSnakes);
        }

        [Fact]
        public void NumLadders_ShouldHandleMultiple()
        {
            // Arrange
            var round = new SnakeLadderRound();

            // Act
            round.NumLadders = 15;

            // Assert
            Assert.Equal(15, round.NumLadders);
        }

        [Fact]
        public void NumSnakes_ShouldHandleMultiple()
        {
            // Arrange
            var round = new SnakeLadderRound();

            // Act
            round.NumSnakes = 12;

            // Assert
            Assert.Equal(12, round.NumSnakes);
        }

        [Fact]
        public void CorrectMinThrows_ShouldHandleMinimumValue()
        {
            // Arrange
            var round = new SnakeLadderRound();

            // Act
            round.CorrectMinThrows = 1;

            // Assert
            Assert.Equal(1, round.CorrectMinThrows);
        }

        [Fact]
        public void CorrectMinThrows_ShouldHandleLargeValue()
        {
            // Arrange
            var round = new SnakeLadderRound();

            // Act
            round.CorrectMinThrows = 100;

            // Assert
            Assert.Equal(100, round.CorrectMinThrows);
        }

        [Fact]
        public void DatePlayed_ShouldAcceptVariousDateTimeFormats()
        {
            // Arrange
            var round = new SnakeLadderRound();
            var customFormat = "2024-12-25 18:45:30";

            // Act
            round.DatePlayed = customFormat;

            // Assert
            Assert.Equal(customFormat, round.DatePlayed);
        }

        [Fact]
        public void BoardConfigs_ShouldHandleEmptyCollection()
        {
            // Arrange
            var round = new SnakeLadderRound();

            // Act
            var isEmpty = !round.BoardConfigs.Any();

            // Assert
            Assert.True(isEmpty);
            Assert.Empty(round.BoardConfigs);
        }

        [Fact]
        public void AlgorithmTimes_ShouldHandleEmptyCollection()
        {
            // Arrange
            var round = new SnakeLadderRound();

            // Act
            var isEmpty = !round.AlgorithmTimes.Any();

            // Assert
            Assert.True(isEmpty);
            Assert.Empty(round.AlgorithmTimes);
        }

        #endregion

        #region Domain-Specific Tests

        [Fact]
        public void Round_ShouldTrackStandard100CellBoard()
        {
            // Arrange
            var round = new SnakeLadderRound
            {
                BoardSize_N = 100,
                NumLadders = 10,
                NumSnakes = 10
            };

            // Act & Assert
            Assert.Equal(100, round.BoardSize_N);
            Assert.Equal(10, round.NumLadders);
            Assert.Equal(10, round.NumSnakes);
        }

        [Fact]
        public void Round_ShouldSupportMultipleAlgorithmComparisons()
        {
            // Arrange
            var round = new SnakeLadderRound { RoundID = 1 };
            var bfs = new SnakeLadderAlgoTime
            {
                AlgorithmName = "BFS",
                TimeTaken_ms = 2.5
            };
            var dijkstra = new SnakeLadderAlgoTime
            {
                AlgorithmName = "Dijkstra",
                TimeTaken_ms = 3.0
            };

            // Act
            round.AlgorithmTimes.Add(bfs);
            round.AlgorithmTimes.Add(dijkstra);

            // Assert
            Assert.Equal(2, round.AlgorithmTimes.Count);
            Assert.Contains(round.AlgorithmTimes, a => a.AlgorithmName == "BFS");
            Assert.Contains(round.AlgorithmTimes, a => a.AlgorithmName == "Dijkstra");
        }

        [Fact]
        public void Round_ShouldSupportBalancedBoardConfiguration()
        {
            // Arrange
            var round = new SnakeLadderRound
            {
                BoardSize_N = 100,
                NumLadders = 8,
                NumSnakes = 8
            };

            // Act
            var isBalanced = round.NumLadders == round.NumSnakes;

            // Assert
            Assert.True(isBalanced);
        }

        [Fact]
        public void BoardConfigs_ShouldDistinguishBetweenLaddersAndSnakes()
        {
            // Arrange
            var round = new SnakeLadderRound { RoundID = 1 };
            var ladder = new SnakeLadderBoardConfig
            {
                FeatureType = "Ladder",
                Start_Cell = 5,
                End_Cell = 20
            };
            var snake = new SnakeLadderBoardConfig
            {
                FeatureType = "Snake",
                Start_Cell = 25,
                End_Cell = 10
            };

            // Act
            round.BoardConfigs.Add(ladder);
            round.BoardConfigs.Add(snake);

            // Assert
            Assert.Single(round.BoardConfigs.Where(c => c.FeatureType == "Ladder"));
            Assert.Single(round.BoardConfigs.Where(c => c.FeatureType == "Snake"));
        }

        #endregion
    }
}
