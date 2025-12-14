using System.ComponentModel.DataAnnotations;
using PDSA.API.Data.Models;
using PDSA.API.Data.Models.EightQueens;

namespace PDSA.Tests.Data.Models.EightQueens
{
    public class EQPSolutionTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var solution = new EQPSolution();

            // Assert
            Assert.Equal(0, solution.SolutionID);
            Assert.Null(solution.PlayerID);
            Assert.Null(solution.DateFound);
            Assert.Equal(string.Empty, solution.Solution_Text);
            Assert.False(solution.IsFound);
            Assert.Null(solution.Player);
        }

        [Fact]
        public void Properties_ShouldSetAndGetValues()
        {
            // Arrange
            var solution = new EQPSolution();
            var solutionText = "[0,4,7,5,2,6,1,3]";
            var dateTime = "2024-01-15T10:30:00";

            // Act
            solution.SolutionID = 123;
            solution.PlayerID = 456;
            solution.DateFound = dateTime;
            solution.Solution_Text = solutionText;
            solution.IsFound = true;

            // Assert
            Assert.Equal(123, solution.SolutionID);
            Assert.Equal(456, solution.PlayerID);
            Assert.Equal(dateTime, solution.DateFound);
            Assert.Equal(solutionText, solution.Solution_Text);
            Assert.True(solution.IsFound);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void SolutionID_ShouldHaveKeyAttribute()
        {
            // Arrange
            var property = typeof(EQPSolution).GetProperty(nameof(EQPSolution.SolutionID));

            // Act
            var keyAttribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(keyAttribute);
        }

        [Fact]
        public void Solution_Text_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(EQPSolution).GetProperty(nameof(EQPSolution.Solution_Text));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void IsFound_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(EQPSolution).GetProperty(nameof(EQPSolution.IsFound));

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
            var solution = new EQPSolution();
            var player = new Player
            {
                PlayerID = 1,
                Name = "TestPlayer"
            };

            // Act
            solution.Player = player;

            // Assert
            Assert.NotNull(solution.Player);
            Assert.Equal(1, solution.Player.PlayerID);
            Assert.Equal("TestPlayer", solution.Player.Name);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void PlayerID_ShouldAcceptNull()
        {
            // Arrange
            var solution = new EQPSolution();

            // Act
            solution.PlayerID = null;

            // Assert
            Assert.Null(solution.PlayerID);
        }

        [Fact]
        public void DateFound_ShouldAcceptNull()
        {
            // Arrange
            var solution = new EQPSolution();

            // Act
            solution.DateFound = null;

            // Assert
            Assert.Null(solution.DateFound);
        }

        [Fact]
        public void DateFound_ShouldAcceptValidDateTime()
        {
            // Arrange
            var solution = new EQPSolution();
            var dateTime = "2024-03-15T14:30:00Z";

            // Act
            solution.DateFound = dateTime;

            // Assert
            Assert.Equal(dateTime, solution.DateFound);
        }

        [Fact]
        public void Solution_Text_ShouldHandleArrayFormat()
        {
            // Arrange
            var solution = new EQPSolution();
            var arrayFormat = "[0,4,7,5,2,6,1,3]";

            // Act
            solution.Solution_Text = arrayFormat;

            // Assert
            Assert.Equal(arrayFormat, solution.Solution_Text);
        }

        [Fact]
        public void Solution_Text_ShouldHandleCommaDelimitedFormat()
        {
            // Arrange
            var solution = new EQPSolution();
            var commaFormat = "0,4,7,5,2,6,1,3";

            // Act
            solution.Solution_Text = commaFormat;

            // Assert
            Assert.Equal(commaFormat, solution.Solution_Text);
        }

        [Fact]
        public void Solution_Text_ShouldHandleLongSolution()
        {
            // Arrange
            var solution = new EQPSolution();
            var longSolution = string.Join(",", Enumerable.Range(0, 100));

            // Act
            solution.Solution_Text = longSolution;

            // Assert
            Assert.Equal(longSolution, solution.Solution_Text);
        }

        [Fact]
        public void IsFound_ShouldDefaultToFalse()
        {
            // Arrange & Act
            var solution = new EQPSolution();

            // Assert
            Assert.False(solution.IsFound);
        }

        [Fact]
        public void IsFound_ShouldToggleBetweenTrueAndFalse()
        {
            // Arrange
            var solution = new EQPSolution();

            // Act & Assert
            solution.IsFound = true;
            Assert.True(solution.IsFound);

            solution.IsFound = false;
            Assert.False(solution.IsFound);
        }

        #endregion

        #region Domain-Specific Tests

        [Fact]
        public void Solution_ShouldRepresentValidQueenPlacement()
        {
            // Arrange
            var solution = new EQPSolution
            {
                Solution_Text = "[0,4,7,5,2,6,1,3]",
                IsFound = true
            };

            // Act
            var positions = solution.Solution_Text.Trim('[', ']').Split(',').Select(int.Parse).ToArray();

            // Assert
            Assert.Equal(8, positions.Length);
            Assert.All(positions, p => Assert.InRange(p, 0, 7));
        }

        [Fact]
        public void Solution_ShouldSupportDifferentBoardSizes()
        {
            // Arrange
            var solution4x4 = new EQPSolution { Solution_Text = "[1,3,0,2]" };
            var solution8x8 = new EQPSolution { Solution_Text = "[0,4,7,5,2,6,1,3]" };

            // Act
            var positions4 = solution4x4.Solution_Text.Trim('[', ']').Split(',').Length;
            var positions8 = solution8x8.Solution_Text.Trim('[', ']').Split(',').Length;

            // Assert
            Assert.Equal(4, positions4);
            Assert.Equal(8, positions8);
        }

        [Fact]
        public void Solution_ShouldTrackDiscoveryStatus()
        {
            // Arrange
            var undiscovered = new EQPSolution { IsFound = false };
            var discovered = new EQPSolution { IsFound = true };

            // Act & Assert
            Assert.False(undiscovered.IsFound);
            Assert.True(discovered.IsFound);
        }

        [Fact]
        public void Solution_ShouldAssociateWithPlayer()
        {
            // Arrange
            var player = new Player { PlayerID = 1, Name = "Alice" };
            var solution = new EQPSolution
            {
                PlayerID = 1,
                Player = player,
                Solution_Text = "[0,4,7,5,2,6,1,3]",
                IsFound = true,
                DateFound = "2024-01-15T10:30:00"
            };

            // Act & Assert
            Assert.Equal(player.PlayerID, solution.PlayerID);
            Assert.Equal(player, solution.Player);
            Assert.NotNull(solution.DateFound);
        }

        [Fact]
        public void Solution_ShouldSupportUnassociatedSolutions()
        {
            // Arrange
            var solution = new EQPSolution
            {
                Solution_Text = "[0,4,7,5,2,6,1,3]",
                IsFound = false,
                PlayerID = null,
                DateFound = null
            };

            // Act & Assert
            Assert.Null(solution.PlayerID);
            Assert.Null(solution.DateFound);
            Assert.False(solution.IsFound);
        }

        [Fact]
        public void MultipleSolutions_ShouldBeDistinguishableByID()
        {
            // Arrange
            var solution1 = new EQPSolution
            {
                SolutionID = 1,
                Solution_Text = "[0,4,7,5,2,6,1,3]"
            };
            var solution2 = new EQPSolution
            {
                SolutionID = 2,
                Solution_Text = "[0,5,7,2,6,3,1,4]"
            };

            // Act
            var solutions = new List<EQPSolution> { solution1, solution2 };

            // Assert
            Assert.Equal(2, solutions.Count);
            Assert.NotEqual(solution1.SolutionID, solution2.SolutionID);
            Assert.NotEqual(solution1.Solution_Text, solution2.Solution_Text);
        }

        [Fact]
        public void Solution_ShouldSupportTimestampTracking()
        {
            // Arrange
            var solution = new EQPSolution
            {
                DateFound = "2024-01-15 10:30:00",
                IsFound = true
            };

            // Act & Assert
            Assert.NotNull(solution.DateFound);
            Assert.True(solution.IsFound);
        }

        #endregion
    }
}
