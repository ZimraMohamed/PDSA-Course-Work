using System.ComponentModel.DataAnnotations;
using PDSA.API.Data.Models.SnakeAndLadder;

namespace PDSA.Tests.Data.Models.SnakeAndLadder
{
    public class SnakeLadderBoardConfigTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var config = new SnakeLadderBoardConfig();

            // Assert
            Assert.Equal(0, config.ConfigID);
            Assert.Equal(0, config.RoundID);
            Assert.Equal(string.Empty, config.FeatureType);
            Assert.Equal(0, config.Start_Cell);
            Assert.Equal(0, config.End_Cell);
            Assert.Null(config.Round);
        }

        [Fact]
        public void Properties_ShouldSetAndGetValues()
        {
            // Arrange
            var config = new SnakeLadderBoardConfig();

            // Act
            config.ConfigID = 123;
            config.RoundID = 456;
            config.FeatureType = "Ladder";
            config.Start_Cell = 5;
            config.End_Cell = 20;

            // Assert
            Assert.Equal(123, config.ConfigID);
            Assert.Equal(456, config.RoundID);
            Assert.Equal("Ladder", config.FeatureType);
            Assert.Equal(5, config.Start_Cell);
            Assert.Equal(20, config.End_Cell);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void ConfigID_ShouldHaveKeyAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderBoardConfig).GetProperty(nameof(SnakeLadderBoardConfig.ConfigID));

            // Act
            var keyAttribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(keyAttribute);
        }

        [Fact]
        public void RoundID_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderBoardConfig).GetProperty(nameof(SnakeLadderBoardConfig.RoundID));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void FeatureType_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderBoardConfig).GetProperty(nameof(SnakeLadderBoardConfig.FeatureType));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void FeatureType_ShouldHaveMaxLengthAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderBoardConfig).GetProperty(nameof(SnakeLadderBoardConfig.FeatureType));

            // Act
            var maxLengthAttribute = property?.GetCustomAttributes(typeof(MaxLengthAttribute), false)
                .FirstOrDefault() as MaxLengthAttribute;

            // Assert
            Assert.NotNull(maxLengthAttribute);
            Assert.Equal(10, maxLengthAttribute.Length);
        }

        [Fact]
        public void Start_Cell_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderBoardConfig).GetProperty(nameof(SnakeLadderBoardConfig.Start_Cell));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void End_Cell_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderBoardConfig).GetProperty(nameof(SnakeLadderBoardConfig.End_Cell));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        #endregion

        #region Navigation Property Tests

        [Fact]
        public void Round_ShouldSetAndGetValue()
        {
            // Arrange
            var config = new SnakeLadderBoardConfig();
            var round = new SnakeLadderRound
            {
                RoundID = 1,
                BoardSize_N = 100,
                NumLadders = 10,
                NumSnakes = 8
            };

            // Act
            config.Round = round;

            // Assert
            Assert.NotNull(config.Round);
            Assert.Equal(1, config.Round.RoundID);
            Assert.Equal(100, config.Round.BoardSize_N);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void FeatureType_ShouldAcceptLadder()
        {
            // Arrange
            var config = new SnakeLadderBoardConfig();

            // Act
            config.FeatureType = "Ladder";

            // Assert
            Assert.Equal("Ladder", config.FeatureType);
        }

        [Fact]
        public void FeatureType_ShouldAcceptSnake()
        {
            // Arrange
            var config = new SnakeLadderBoardConfig();

            // Act
            config.FeatureType = "Snake";

            // Assert
            Assert.Equal("Snake", config.FeatureType);
        }

        [Fact]
        public void FeatureType_ShouldHandleLowercaseVariants()
        {
            // Arrange
            var config = new SnakeLadderBoardConfig();

            // Act
            config.FeatureType = "ladder";

            // Assert
            Assert.Equal("ladder", config.FeatureType);
        }

        [Fact]
        public void Start_Cell_ShouldHandleBoardStart()
        {
            // Arrange
            var config = new SnakeLadderBoardConfig();

            // Act
            config.Start_Cell = 1;

            // Assert
            Assert.Equal(1, config.Start_Cell);
        }

        [Fact]
        public void Start_Cell_ShouldHandleMidBoardValues()
        {
            // Arrange
            var config = new SnakeLadderBoardConfig();

            // Act
            config.Start_Cell = 50;

            // Assert
            Assert.Equal(50, config.Start_Cell);
        }

        [Fact]
        public void End_Cell_ShouldHandleBoardEnd()
        {
            // Arrange
            var config = new SnakeLadderBoardConfig();

            // Act
            config.End_Cell = 100;

            // Assert
            Assert.Equal(100, config.End_Cell);
        }

        [Fact]
        public void End_Cell_ShouldHandleMidBoardValues()
        {
            // Arrange
            var config = new SnakeLadderBoardConfig();

            // Act
            config.End_Cell = 45;

            // Assert
            Assert.Equal(45, config.End_Cell);
        }

        #endregion

        #region Domain-Specific Tests

        [Fact]
        public void Ladder_ShouldHaveEndCellGreaterThanStartCell()
        {
            // Arrange
            var ladder = new SnakeLadderBoardConfig
            {
                FeatureType = "Ladder",
                Start_Cell = 5,
                End_Cell = 20
            };

            // Act & Assert
            Assert.True(ladder.End_Cell > ladder.Start_Cell);
        }

        [Fact]
        public void Snake_ShouldHaveEndCellLessThanStartCell()
        {
            // Arrange
            var snake = new SnakeLadderBoardConfig
            {
                FeatureType = "Snake",
                Start_Cell = 25,
                End_Cell = 10
            };

            // Act & Assert
            Assert.True(snake.End_Cell < snake.Start_Cell);
        }

        [Fact]
        public void BoardConfig_ShouldCalculateLadderLength()
        {
            // Arrange
            var ladder = new SnakeLadderBoardConfig
            {
                FeatureType = "Ladder",
                Start_Cell = 5,
                End_Cell = 20
            };

            // Act
            var length = ladder.End_Cell - ladder.Start_Cell;

            // Assert
            Assert.Equal(15, length);
        }

        [Fact]
        public void BoardConfig_ShouldCalculateSnakeLength()
        {
            // Arrange
            var snake = new SnakeLadderBoardConfig
            {
                FeatureType = "Snake",
                Start_Cell = 30,
                End_Cell = 10
            };

            // Act
            var length = snake.Start_Cell - snake.End_Cell;

            // Assert
            Assert.Equal(20, length);
        }

        [Fact]
        public void BoardConfig_ShouldSupportLongLadders()
        {
            // Arrange
            var longLadder = new SnakeLadderBoardConfig
            {
                FeatureType = "Ladder",
                Start_Cell = 3,
                End_Cell = 95
            };

            // Act
            var length = longLadder.End_Cell - longLadder.Start_Cell;

            // Assert
            Assert.True(length > 50);
        }

        [Fact]
        public void BoardConfig_ShouldSupportShortLadders()
        {
            // Arrange
            var shortLadder = new SnakeLadderBoardConfig
            {
                FeatureType = "Ladder",
                Start_Cell = 10,
                End_Cell = 12
            };

            // Act
            var length = shortLadder.End_Cell - shortLadder.Start_Cell;

            // Assert
            Assert.True(length <= 5);
        }

        [Fact]
        public void MultipleConfigs_ShouldSupportSameRound()
        {
            // Arrange
            var configs = new List<SnakeLadderBoardConfig>
            {
                new SnakeLadderBoardConfig { RoundID = 1, FeatureType = "Ladder", Start_Cell = 5, End_Cell = 20 },
                new SnakeLadderBoardConfig { RoundID = 1, FeatureType = "Snake", Start_Cell = 25, End_Cell = 10 },
                new SnakeLadderBoardConfig { RoundID = 1, FeatureType = "Ladder", Start_Cell = 30, End_Cell = 50 }
            };

            // Act
            var uniqueRounds = configs.Select(c => c.RoundID).Distinct().Count();

            // Assert
            Assert.Equal(1, uniqueRounds);
            Assert.Equal(2, configs.Count(c => c.FeatureType == "Ladder"));
            Assert.Single(configs.Where(c => c.FeatureType == "Snake"));
        }

        #endregion
    }
}
