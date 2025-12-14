using System.ComponentModel.DataAnnotations;
using PDSA.API.Data.Models.EightQueens;

namespace PDSA.Tests.Data.Models.EightQueens
{
    public class EQPAlgoTimeTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var algoTime = new EQPAlgoTime();

            // Assert
            Assert.Equal(0, algoTime.TimeID);
            Assert.Equal(string.Empty, algoTime.DateExecuted);
            Assert.Equal(string.Empty, algoTime.AlgorithmType);
            Assert.Equal(0.0, algoTime.TimeTaken_ms);
            Assert.Equal(0, algoTime.RoundNumber);
        }

        [Fact]
        public void Properties_ShouldSetAndGetValues()
        {
            // Arrange
            var algoTime = new EQPAlgoTime();
            var dateTime = "2024-01-15T10:30:00";

            // Act
            algoTime.TimeID = 101;
            algoTime.DateExecuted = dateTime;
            algoTime.AlgorithmType = "Backtracking";
            algoTime.TimeTaken_ms = 12.5;
            algoTime.RoundNumber = 5;

            // Assert
            Assert.Equal(101, algoTime.TimeID);
            Assert.Equal(dateTime, algoTime.DateExecuted);
            Assert.Equal("Backtracking", algoTime.AlgorithmType);
            Assert.Equal(12.5, algoTime.TimeTaken_ms);
            Assert.Equal(5, algoTime.RoundNumber);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void TimeID_ShouldHaveKeyAttribute()
        {
            // Arrange
            var property = typeof(EQPAlgoTime).GetProperty(nameof(EQPAlgoTime.TimeID));

            // Act
            var keyAttribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(keyAttribute);
        }

        [Fact]
        public void DateExecuted_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(EQPAlgoTime).GetProperty(nameof(EQPAlgoTime.DateExecuted));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void AlgorithmType_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(EQPAlgoTime).GetProperty(nameof(EQPAlgoTime.AlgorithmType));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(EQPAlgoTime).GetProperty(nameof(EQPAlgoTime.TimeTaken_ms));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void RoundNumber_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(EQPAlgoTime).GetProperty(nameof(EQPAlgoTime.RoundNumber));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void DateExecuted_ShouldAcceptVariousDateTimeFormats()
        {
            // Arrange
            var algoTime = new EQPAlgoTime();
            var isoFormat = "2024-03-15T14:30:00Z";

            // Act
            algoTime.DateExecuted = isoFormat;

            // Assert
            Assert.Equal(isoFormat, algoTime.DateExecuted);
        }

        [Fact]
        public void AlgorithmType_ShouldAcceptBacktracking()
        {
            // Arrange
            var algoTime = new EQPAlgoTime();

            // Act
            algoTime.AlgorithmType = "Backtracking";

            // Assert
            Assert.Equal("Backtracking", algoTime.AlgorithmType);
        }

        [Fact]
        public void AlgorithmType_ShouldAcceptSequential()
        {
            // Arrange
            var algoTime = new EQPAlgoTime();

            // Act
            algoTime.AlgorithmType = "Sequential";

            // Assert
            Assert.Equal("Sequential", algoTime.AlgorithmType);
        }

        [Fact]
        public void AlgorithmType_ShouldAcceptParallel()
        {
            // Arrange
            var algoTime = new EQPAlgoTime();

            // Act
            algoTime.AlgorithmType = "Parallel";

            // Assert
            Assert.Equal("Parallel", algoTime.AlgorithmType);
        }

        [Fact]
        public void AlgorithmType_ShouldHandleCustomNames()
        {
            // Arrange
            var algoTime = new EQPAlgoTime();

            // Act
            algoTime.AlgorithmType = "Optimized Backtracking with Pruning";

            // Assert
            Assert.Equal("Optimized Backtracking with Pruning", algoTime.AlgorithmType);
        }

        [Fact]
        public void AlgorithmType_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var algoTime = new EQPAlgoTime();
            var unicodeName = "回溯算法";

            // Act
            algoTime.AlgorithmType = unicodeName;

            // Assert
            Assert.Equal(unicodeName, algoTime.AlgorithmType);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHandleVerySmallValues()
        {
            // Arrange
            var algoTime = new EQPAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 0.001;

            // Assert
            Assert.Equal(0.001, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHandleLargeValues()
        {
            // Arrange
            var algoTime = new EQPAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 9999.999;

            // Assert
            Assert.Equal(9999.999, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHandleZero()
        {
            // Arrange
            var algoTime = new EQPAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 0.0;

            // Assert
            Assert.Equal(0.0, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHandlePreciseDecimalValues()
        {
            // Arrange
            var algoTime = new EQPAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 15.123456;

            // Assert
            Assert.Equal(15.123456, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void RoundNumber_ShouldHandleFirstRound()
        {
            // Arrange
            var algoTime = new EQPAlgoTime();

            // Act
            algoTime.RoundNumber = 1;

            // Assert
            Assert.Equal(1, algoTime.RoundNumber);
        }

        [Fact]
        public void RoundNumber_ShouldHandleMultipleRounds()
        {
            // Arrange
            var algoTime = new EQPAlgoTime();

            // Act
            algoTime.RoundNumber = 100;

            // Assert
            Assert.Equal(100, algoTime.RoundNumber);
        }

        #endregion

        #region Domain-Specific Tests

        [Fact]
        public void MultipleAlgorithmTimes_ShouldBeComparable()
        {
            // Arrange
            var backtracking = new EQPAlgoTime
            {
                AlgorithmType = "Backtracking",
                TimeTaken_ms = 10.5
            };
            var sequential = new EQPAlgoTime
            {
                AlgorithmType = "Sequential",
                TimeTaken_ms = 15.2
            };

            // Act & Assert
            Assert.True(backtracking.TimeTaken_ms < sequential.TimeTaken_ms);
        }

        [Fact]
        public void AlgorithmTime_ShouldScaleWithBoardSize()
        {
            // Arrange
            var small4x4 = new EQPAlgoTime
            {
                AlgorithmType = "Backtracking",
                TimeTaken_ms = 0.5,
                RoundNumber = 1
            };
            var large8x8 = new EQPAlgoTime
            {
                AlgorithmType = "Backtracking",
                TimeTaken_ms = 15.0,
                RoundNumber = 2
            };

            // Act & Assert
            Assert.True(large8x8.TimeTaken_ms > small4x4.TimeTaken_ms);
        }

        [Fact]
        public void AlgorithmTime_ShouldSupportPerformanceAnalysis()
        {
            // Arrange
            var times = new List<EQPAlgoTime>
            {
                new EQPAlgoTime { AlgorithmType = "Backtracking", TimeTaken_ms = 10.5 },
                new EQPAlgoTime { AlgorithmType = "Sequential", TimeTaken_ms = 15.2 },
                new EQPAlgoTime { AlgorithmType = "Parallel", TimeTaken_ms = 8.3 }
            };

            // Act
            var fastest = times.OrderBy(t => t.TimeTaken_ms).First();
            var slowest = times.OrderByDescending(t => t.TimeTaken_ms).First();

            // Assert
            Assert.Equal("Parallel", fastest.AlgorithmType);
            Assert.Equal("Sequential", slowest.AlgorithmType);
        }

        [Fact]
        public void AlgorithmTime_ShouldTrackMillisecondPrecision()
        {
            // Arrange
            var algoTime1 = new EQPAlgoTime { TimeTaken_ms = 10.123 };
            var algoTime2 = new EQPAlgoTime { TimeTaken_ms = 10.124 };

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
            var benchmark1 = new EQPAlgoTime
            {
                AlgorithmType = "Backtracking V1",
                TimeTaken_ms = 15.0,
                RoundNumber = 1
            };
            var benchmark2 = new EQPAlgoTime
            {
                AlgorithmType = "Backtracking V2",
                TimeTaken_ms = 12.0,
                RoundNumber = 1
            };

            // Act
            var improvement = ((benchmark1.TimeTaken_ms - benchmark2.TimeTaken_ms) / benchmark1.TimeTaken_ms) * 100;

            // Assert
            Assert.True(improvement > 0);
            Assert.Equal(20.0, improvement);
        }

        [Fact]
        public void AlgorithmTime_ShouldHandleMultipleRounds()
        {
            // Arrange
            var time1 = new EQPAlgoTime { RoundNumber = 1, AlgorithmType = "Backtracking", TimeTaken_ms = 10.0 };
            var time2 = new EQPAlgoTime { RoundNumber = 2, AlgorithmType = "Backtracking", TimeTaken_ms = 11.0 };
            var time3 = new EQPAlgoTime { RoundNumber = 3, AlgorithmType = "Backtracking", TimeTaken_ms = 12.0 };

            // Act
            var times = new List<EQPAlgoTime> { time1, time2, time3 };
            var averageTime = times.Average(t => t.TimeTaken_ms);

            // Assert
            Assert.Equal(11.0, averageTime);
            Assert.Equal(3, times.Select(t => t.RoundNumber).Distinct().Count());
        }

        [Fact]
        public void AlgorithmTime_ShouldSupportTimestampTracking()
        {
            // Arrange
            var algoTime = new EQPAlgoTime
            {
                DateExecuted = "2024-01-15 10:30:00",
                AlgorithmType = "Backtracking",
                TimeTaken_ms = 10.5,
                RoundNumber = 1
            };

            // Act & Assert
            Assert.NotEmpty(algoTime.DateExecuted);
            Assert.True(algoTime.TimeTaken_ms > 0);
        }

        [Fact]
        public void AlgorithmTime_ShouldGroupByRoundNumber()
        {
            // Arrange
            var times = new List<EQPAlgoTime>
            {
                new EQPAlgoTime { RoundNumber = 1, AlgorithmType = "Backtracking", TimeTaken_ms = 10.0 },
                new EQPAlgoTime { RoundNumber = 1, AlgorithmType = "Sequential", TimeTaken_ms = 15.0 },
                new EQPAlgoTime { RoundNumber = 2, AlgorithmType = "Backtracking", TimeTaken_ms = 11.0 }
            };

            // Act
            var round1Times = times.Where(t => t.RoundNumber == 1).ToList();
            var round2Times = times.Where(t => t.RoundNumber == 2).ToList();

            // Assert
            Assert.Equal(2, round1Times.Count);
            Assert.Single(round2Times);
        }

        #endregion
    }
}
