using System.ComponentModel.DataAnnotations;
using PDSA.API.Data.Models.TowerOfHanoi;

namespace PDSA.Tests.Data.Models.TowerOfHanoi
{
    public class HanoiAlgoTimeTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var algoTime = new HanoiAlgoTime();

            // Assert
            Assert.Equal(0, algoTime.TimeID);
            Assert.Equal(0, algoTime.RoundID);
            Assert.Equal(string.Empty, algoTime.AlgorithmName);
            Assert.Equal(0.0, algoTime.TimeTaken_ms);
            Assert.Null(algoTime.Round);
        }

        [Fact]
        public void Properties_ShouldSetAndGetValues()
        {
            // Arrange
            var algoTime = new HanoiAlgoTime();

            // Act
            algoTime.TimeID = 101;
            algoTime.RoundID = 202;
            algoTime.AlgorithmName = "Recursive";
            algoTime.TimeTaken_ms = 2.5;

            // Assert
            Assert.Equal(101, algoTime.TimeID);
            Assert.Equal(202, algoTime.RoundID);
            Assert.Equal("Recursive", algoTime.AlgorithmName);
            Assert.Equal(2.5, algoTime.TimeTaken_ms);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void TimeID_ShouldHaveKeyAttribute()
        {
            // Arrange
            var property = typeof(HanoiAlgoTime).GetProperty(nameof(HanoiAlgoTime.TimeID));

            // Act
            var keyAttribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(keyAttribute);
        }

        [Fact]
        public void RoundID_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(HanoiAlgoTime).GetProperty(nameof(HanoiAlgoTime.RoundID));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void AlgorithmName_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(HanoiAlgoTime).GetProperty(nameof(HanoiAlgoTime.AlgorithmName));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(HanoiAlgoTime).GetProperty(nameof(HanoiAlgoTime.TimeTaken_ms));

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
            var algoTime = new HanoiAlgoTime();
            var round = new HanoiRound
            {
                RoundID = 1,
                NumDisks_N = 5,
                NumPegs = 3,
                CorrectMoves_Count = 31
            };

            // Act
            algoTime.Round = round;

            // Assert
            Assert.NotNull(algoTime.Round);
            Assert.Equal(1, algoTime.Round.RoundID);
            Assert.Equal(5, algoTime.Round.NumDisks_N);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void AlgorithmName_ShouldAcceptRecursive()
        {
            // Arrange
            var algoTime = new HanoiAlgoTime();

            // Act
            algoTime.AlgorithmName = "Recursive";

            // Assert
            Assert.Equal("Recursive", algoTime.AlgorithmName);
        }

        [Fact]
        public void AlgorithmName_ShouldAcceptIterative()
        {
            // Arrange
            var algoTime = new HanoiAlgoTime();

            // Act
            algoTime.AlgorithmName = "Iterative";

            // Assert
            Assert.Equal("Iterative", algoTime.AlgorithmName);
        }

        [Fact]
        public void AlgorithmName_ShouldAcceptCustomNames()
        {
            // Arrange
            var algoTime = new HanoiAlgoTime();

            // Act
            algoTime.AlgorithmName = "Optimized Recursive";

            // Assert
            Assert.Equal("Optimized Recursive", algoTime.AlgorithmName);
        }

        [Fact]
        public void AlgorithmName_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var algoTime = new HanoiAlgoTime();
            var unicodeName = "递归算法";

            // Act
            algoTime.AlgorithmName = unicodeName;

            // Assert
            Assert.Equal(unicodeName, algoTime.AlgorithmName);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHandleVerySmallValues()
        {
            // Arrange
            var algoTime = new HanoiAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 0.001;

            // Assert
            Assert.Equal(0.001, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHandleLargeValues()
        {
            // Arrange
            var algoTime = new HanoiAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 9999.999;

            // Assert
            Assert.Equal(9999.999, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHandleZero()
        {
            // Arrange
            var algoTime = new HanoiAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 0.0;

            // Assert
            Assert.Equal(0.0, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHandlePreciseDecimalValues()
        {
            // Arrange
            var algoTime = new HanoiAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 1.23456789;

            // Assert
            Assert.Equal(1.23456789, algoTime.TimeTaken_ms);
        }

        #endregion

        #region Domain-Specific Tests

        [Fact]
        public void MultipleAlgorithmTimes_ShouldBeComparable()
        {
            // Arrange
            var recursive = new HanoiAlgoTime
            {
                RoundID = 1,
                AlgorithmName = "Recursive",
                TimeTaken_ms = 1.2
            };
            var iterative = new HanoiAlgoTime
            {
                RoundID = 1,
                AlgorithmName = "Iterative",
                TimeTaken_ms = 1.5
            };

            // Act & Assert
            Assert.True(recursive.TimeTaken_ms < iterative.TimeTaken_ms);
        }

        [Fact]
        public void AlgorithmTime_ShouldScaleWithProblemSize()
        {
            // Arrange
            var smallProblem = new HanoiAlgoTime
            {
                RoundID = 1,
                AlgorithmName = "Recursive",
                TimeTaken_ms = 0.5
            };
            var largeProblem = new HanoiAlgoTime
            {
                RoundID = 2,
                AlgorithmName = "Recursive",
                TimeTaken_ms = 50.0
            };

            // Act & Assert
            Assert.True(largeProblem.TimeTaken_ms > smallProblem.TimeTaken_ms);
        }

        [Fact]
        public void AlgorithmTime_ShouldSupportPerformanceAnalysis()
        {
            // Arrange
            var times = new List<HanoiAlgoTime>
            {
                new HanoiAlgoTime { AlgorithmName = "Recursive", TimeTaken_ms = 1.2 },
                new HanoiAlgoTime { AlgorithmName = "Iterative", TimeTaken_ms = 1.5 },
                new HanoiAlgoTime { AlgorithmName = "Optimized", TimeTaken_ms = 0.8 }
            };

            // Act
            var fastest = times.OrderBy(t => t.TimeTaken_ms).First();
            var slowest = times.OrderByDescending(t => t.TimeTaken_ms).First();

            // Assert
            Assert.Equal("Optimized", fastest.AlgorithmName);
            Assert.Equal("Iterative", slowest.AlgorithmName);
        }

        [Fact]
        public void AlgorithmTime_ShouldTrackMillisecondPrecision()
        {
            // Arrange
            var algoTime1 = new HanoiAlgoTime { TimeTaken_ms = 1.234 };
            var algoTime2 = new HanoiAlgoTime { TimeTaken_ms = 1.235 };

            // Act
            var difference = algoTime2.TimeTaken_ms - algoTime1.TimeTaken_ms;

            // Assert
            Assert.True(difference > 0);
            Assert.True(difference < 0.01);
        }

        [Fact]
        public void AlgorithmTime_ShouldSupportBenchmarking()
        {
            // Arrange
            var round = new HanoiRound { RoundID = 1, NumDisks_N = 8 };
            var benchmark1 = new HanoiAlgoTime
            {
                RoundID = 1,
                AlgorithmName = "Recursive V1",
                TimeTaken_ms = 5.2,
                Round = round
            };
            var benchmark2 = new HanoiAlgoTime
            {
                RoundID = 1,
                AlgorithmName = "Recursive V2",
                TimeTaken_ms = 4.8,
                Round = round
            };

            // Act
            var improvement = ((benchmark1.TimeTaken_ms - benchmark2.TimeTaken_ms) / benchmark1.TimeTaken_ms) * 100;

            // Assert
            Assert.True(improvement > 0);
            Assert.Equal(round.RoundID, benchmark1.RoundID);
            Assert.Equal(round.RoundID, benchmark2.RoundID);
        }

        [Fact]
        public void AlgorithmTime_ShouldHandleMultipleRounds()
        {
            // Arrange
            var time1 = new HanoiAlgoTime { RoundID = 1, AlgorithmName = "Recursive", TimeTaken_ms = 1.0 };
            var time2 = new HanoiAlgoTime { RoundID = 2, AlgorithmName = "Recursive", TimeTaken_ms = 2.0 };
            var time3 = new HanoiAlgoTime { RoundID = 3, AlgorithmName = "Recursive", TimeTaken_ms = 3.0 };

            // Act
            var times = new List<HanoiAlgoTime> { time1, time2, time3 };
            var averageTime = times.Average(t => t.TimeTaken_ms);

            // Assert
            Assert.Equal(2.0, averageTime);
            Assert.Equal(3, times.Select(t => t.RoundID).Distinct().Count());
        }

        #endregion
    }
}
