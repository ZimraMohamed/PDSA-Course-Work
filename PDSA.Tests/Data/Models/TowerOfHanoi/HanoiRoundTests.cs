using System.ComponentModel.DataAnnotations;
using PDSA.API.Data.Models;
using PDSA.API.Data.Models.TowerOfHanoi;

namespace PDSA.Tests.Data.Models.TowerOfHanoi
{
    public class HanoiRoundTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var round = new HanoiRound();

            // Assert
            Assert.Equal(0, round.RoundID);
            Assert.Equal(0, round.PlayerID);
            Assert.Equal(0, round.NumDisks_N);
            Assert.Equal(0, round.NumPegs);
            Assert.Equal(0, round.CorrectMoves_Count);
            Assert.Equal(string.Empty, round.CorrectMoves_Sequence);
            Assert.Equal(string.Empty, round.DatePlayed);
            Assert.Null(round.Player);
            Assert.NotNull(round.AlgorithmTimes);
            Assert.Empty(round.AlgorithmTimes);
        }

        [Fact]
        public void Properties_ShouldSetAndGetValues()
        {
            // Arrange
            var round = new HanoiRound();
            var moves = "A->C, A->B, C->B, A->C, B->A, B->C, A->C";
            var dateTime = "2024-01-15T10:30:00";

            // Act
            round.RoundID = 123;
            round.PlayerID = 456;
            round.NumDisks_N = 3;
            round.NumPegs = 3;
            round.CorrectMoves_Count = 7;
            round.CorrectMoves_Sequence = moves;
            round.DatePlayed = dateTime;

            // Assert
            Assert.Equal(123, round.RoundID);
            Assert.Equal(456, round.PlayerID);
            Assert.Equal(3, round.NumDisks_N);
            Assert.Equal(3, round.NumPegs);
            Assert.Equal(7, round.CorrectMoves_Count);
            Assert.Equal(moves, round.CorrectMoves_Sequence);
            Assert.Equal(dateTime, round.DatePlayed);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void RoundID_ShouldHaveKeyAttribute()
        {
            // Arrange
            var property = typeof(HanoiRound).GetProperty(nameof(HanoiRound.RoundID));

            // Act
            var keyAttribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(keyAttribute);
        }

        [Fact]
        public void PlayerID_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(HanoiRound).GetProperty(nameof(HanoiRound.PlayerID));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void NumDisks_N_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(HanoiRound).GetProperty(nameof(HanoiRound.NumDisks_N));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void NumPegs_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(HanoiRound).GetProperty(nameof(HanoiRound.NumPegs));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void CorrectMoves_Count_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(HanoiRound).GetProperty(nameof(HanoiRound.CorrectMoves_Count));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void CorrectMoves_Sequence_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(HanoiRound).GetProperty(nameof(HanoiRound.CorrectMoves_Sequence));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void DatePlayed_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(HanoiRound).GetProperty(nameof(HanoiRound.DatePlayed));

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
            var round = new HanoiRound();
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
        public void AlgorithmTimes_ShouldAddMultipleItems()
        {
            // Arrange
            var round = new HanoiRound { RoundID = 1 };
            var time1 = new HanoiAlgoTime
            {
                TimeID = 1,
                RoundID = 1,
                AlgorithmName = "Recursive",
                TimeTaken_ms = 0.5
            };
            var time2 = new HanoiAlgoTime
            {
                TimeID = 2,
                RoundID = 1,
                AlgorithmName = "Iterative",
                TimeTaken_ms = 0.7
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
        public void NumDisks_N_ShouldHandleSmallValue()
        {
            // Arrange
            var round = new HanoiRound();

            // Act
            round.NumDisks_N = 1;

            // Assert
            Assert.Equal(1, round.NumDisks_N);
        }

        [Fact]
        public void NumDisks_N_ShouldHandleLargeValue()
        {
            // Arrange
            var round = new HanoiRound();

            // Act
            round.NumDisks_N = 20;

            // Assert
            Assert.Equal(20, round.NumDisks_N);
        }

        [Fact]
        public void NumPegs_ShouldHandleStandard3Pegs()
        {
            // Arrange
            var round = new HanoiRound();

            // Act
            round.NumPegs = 3;

            // Assert
            Assert.Equal(3, round.NumPegs);
        }

        [Fact]
        public void NumPegs_ShouldHandleMoreThan3Pegs()
        {
            // Arrange
            var round = new HanoiRound();

            // Act
            round.NumPegs = 4;

            // Assert
            Assert.Equal(4, round.NumPegs);
        }

        [Fact]
        public void CorrectMoves_Count_ShouldHandleMinimalMoves()
        {
            // Arrange (For 1 disk, minimum moves = 1)
            var round = new HanoiRound();

            // Act
            round.CorrectMoves_Count = 1;

            // Assert
            Assert.Equal(1, round.CorrectMoves_Count);
        }

        [Fact]
        public void CorrectMoves_Count_ShouldHandleLargeMoves()
        {
            // Arrange (For 10 disks, minimum moves = 1023)
            var round = new HanoiRound();

            // Act
            round.CorrectMoves_Count = 1023;

            // Assert
            Assert.Equal(1023, round.CorrectMoves_Count);
        }

        [Fact]
        public void CorrectMoves_Sequence_ShouldHandleShortSequence()
        {
            // Arrange
            var round = new HanoiRound();
            var shortSequence = "A->C";

            // Act
            round.CorrectMoves_Sequence = shortSequence;

            // Assert
            Assert.Equal(shortSequence, round.CorrectMoves_Sequence);
        }

        [Fact]
        public void CorrectMoves_Sequence_ShouldHandleLongSequence()
        {
            // Arrange
            var round = new HanoiRound();
            var longSequence = string.Join(", ", Enumerable.Repeat("A->C", 100));

            // Act
            round.CorrectMoves_Sequence = longSequence;

            // Assert
            Assert.Equal(longSequence, round.CorrectMoves_Sequence);
        }

        [Fact]
        public void DatePlayed_ShouldAcceptVariousDateTimeFormats()
        {
            // Arrange
            var round = new HanoiRound();
            var isoFormat = "2024-03-15T14:30:00Z";

            // Act
            round.DatePlayed = isoFormat;

            // Assert
            Assert.Equal(isoFormat, round.DatePlayed);
        }

        [Fact]
        public void AlgorithmTimes_ShouldHandleEmptyCollection()
        {
            // Arrange
            var round = new HanoiRound();

            // Act
            var isEmpty = !round.AlgorithmTimes.Any();

            // Assert
            Assert.True(isEmpty);
            Assert.Empty(round.AlgorithmTimes);
        }

        #endregion

        #region Domain-Specific Tests

        [Fact]
        public void CorrectMoves_Count_ShouldMatchFormulaFor3Disks()
        {
            // Arrange (For 3 disks on 3 pegs, minimum moves = 2^3 - 1 = 7)
            var round = new HanoiRound
            {
                NumDisks_N = 3,
                NumPegs = 3,
                CorrectMoves_Count = 7
            };

            // Act
            var expectedMoves = Math.Pow(2, round.NumDisks_N) - 1;

            // Assert
            Assert.Equal(expectedMoves, round.CorrectMoves_Count);
        }

        [Fact]
        public void CorrectMoves_Sequence_ShouldContainPegIdentifiers()
        {
            // Arrange
            var round = new HanoiRound
            {
                CorrectMoves_Sequence = "A->C, A->B, C->B, A->C, B->A, B->C, A->C"
            };

            // Act & Assert
            Assert.Contains("A->", round.CorrectMoves_Sequence);
            Assert.Contains("->C", round.CorrectMoves_Sequence);
            Assert.Contains("B", round.CorrectMoves_Sequence);
        }

        [Fact]
        public void Round_ShouldSupportMultipleAlgorithmComparisons()
        {
            // Arrange
            var round = new HanoiRound
            {
                RoundID = 1,
                NumDisks_N = 5
            };
            var recursive = new HanoiAlgoTime
            {
                AlgorithmName = "Recursive",
                TimeTaken_ms = 1.2
            };
            var iterative = new HanoiAlgoTime
            {
                AlgorithmName = "Iterative",
                TimeTaken_ms = 1.5
            };

            // Act
            round.AlgorithmTimes.Add(recursive);
            round.AlgorithmTimes.Add(iterative);

            // Assert
            Assert.Equal(2, round.AlgorithmTimes.Count);
            Assert.True(round.AlgorithmTimes.Any(a => a.AlgorithmName == "Recursive"));
            Assert.True(round.AlgorithmTimes.Any(a => a.AlgorithmName == "Iterative"));
        }

        #endregion
    }
}
