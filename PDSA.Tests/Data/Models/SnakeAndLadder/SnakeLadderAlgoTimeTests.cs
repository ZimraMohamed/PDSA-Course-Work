using System.ComponentModel.DataAnnotations;
using PDSA.API.Data.Models.SnakeAndLadder;

namespace PDSA.Tests.Data.Models.SnakeAndLadder
{
    public class SnakeLadderAlgoTimeTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var algoTime = new SnakeLadderAlgoTime();

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
            var algoTime = new SnakeLadderAlgoTime();

            // Act
            algoTime.TimeID = 101;
            algoTime.RoundID = 202;
            algoTime.AlgorithmName = "BFS";
            algoTime.TimeTaken_ms = 5.75;

            // Assert
            Assert.Equal(101, algoTime.TimeID);
            Assert.Equal(202, algoTime.RoundID);
            Assert.Equal("BFS", algoTime.AlgorithmName);
            Assert.Equal(5.75, algoTime.TimeTaken_ms);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void TimeID_ShouldHaveKeyAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderAlgoTime).GetProperty(nameof(SnakeLadderAlgoTime.TimeID));

            // Act
            var keyAttribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(keyAttribute);
        }

        [Fact]
        public void RoundID_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderAlgoTime).GetProperty(nameof(SnakeLadderAlgoTime.RoundID));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void AlgorithmName_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderAlgoTime).GetProperty(nameof(SnakeLadderAlgoTime.AlgorithmName));

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void AlgorithmName_ShouldHaveMaxLengthAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderAlgoTime).GetProperty(nameof(SnakeLadderAlgoTime.AlgorithmName));

            // Act
            var maxLengthAttribute = property?.GetCustomAttributes(typeof(MaxLengthAttribute), false)
                .FirstOrDefault() as MaxLengthAttribute;

            // Assert
            Assert.NotNull(maxLengthAttribute);
            Assert.Equal(100, maxLengthAttribute.Length);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(SnakeLadderAlgoTime).GetProperty(nameof(SnakeLadderAlgoTime.TimeTaken_ms));

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
            var algoTime = new SnakeLadderAlgoTime();
            var round = new SnakeLadderRound
            {
                RoundID = 1,
                BoardSize_N = 100,
                NumLadders = 10,
                NumSnakes = 8
            };

            // Act
            algoTime.Round = round;

            // Assert
            Assert.NotNull(algoTime.Round);
            Assert.Equal(1, algoTime.Round.RoundID);
            Assert.Equal(100, algoTime.Round.BoardSize_N);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void AlgorithmName_ShouldAcceptBFS()
        {
            // Arrange
            var algoTime = new SnakeLadderAlgoTime();

            // Act
            algoTime.AlgorithmName = "BFS";

            // Assert
            Assert.Equal("BFS", algoTime.AlgorithmName);
        }

        [Fact]
        public void AlgorithmName_ShouldAcceptDijkstra()
        {
            // Arrange
            var algoTime = new SnakeLadderAlgoTime();

            // Act
            algoTime.AlgorithmName = "Dijkstra";

            // Assert
            Assert.Equal("Dijkstra", algoTime.AlgorithmName);
        }

        [Fact]
        public void AlgorithmName_ShouldAcceptDynamicProgramming()
        {
            // Arrange
            var algoTime = new SnakeLadderAlgoTime();

            // Act
            algoTime.AlgorithmName = "Dynamic Programming";

            // Assert
            Assert.Equal("Dynamic Programming", algoTime.AlgorithmName);
        }

        [Fact]
        public void AlgorithmName_ShouldHandleLongNames()
        {
            // Arrange
            var algoTime = new SnakeLadderAlgoTime();
            var longName = "Optimized BFS with Priority Queue";

            // Act
            algoTime.AlgorithmName = longName;

            // Assert
            Assert.Equal(longName, algoTime.AlgorithmName);
        }

        [Fact]
        public void AlgorithmName_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var algoTime = new SnakeLadderAlgoTime();
            var unicodeName = "广度优先搜索";

            // Act
            algoTime.AlgorithmName = unicodeName;

            // Assert
            Assert.Equal(unicodeName, algoTime.AlgorithmName);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHandleVerySmallValues()
        {
            // Arrange
            var algoTime = new SnakeLadderAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 0.001;

            // Assert
            Assert.Equal(0.001, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHandleLargeValues()
        {
            // Arrange
            var algoTime = new SnakeLadderAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 9999.999;

            // Assert
            Assert.Equal(9999.999, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHandleZero()
        {
            // Arrange
            var algoTime = new SnakeLadderAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 0.0;

            // Assert
            Assert.Equal(0.0, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TimeTaken_ms_ShouldHandlePreciseDecimalValues()
        {
            // Arrange
            var algoTime = new SnakeLadderAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 3.14159265;

            // Assert
            Assert.Equal(3.14159265, algoTime.TimeTaken_ms);
        }

        #endregion

        #region Domain-Specific Tests

        [Fact]
        public void MultipleAlgorithmTimes_ShouldBeComparable()
        {
            // Arrange
            var bfs = new SnakeLadderAlgoTime
            {
                RoundID = 1,
                AlgorithmName = "BFS",
                TimeTaken_ms = 2.5
            };
            var dijkstra = new SnakeLadderAlgoTime
            {
                RoundID = 1,
                AlgorithmName = "Dijkstra",
                TimeTaken_ms = 3.2
            };

            // Act & Assert
            Assert.True(bfs.TimeTaken_ms < dijkstra.TimeTaken_ms);
        }

        [Fact]
        public void AlgorithmTime_ShouldScaleWithBoardSize()
        {
            // Arrange
            var smallBoard = new SnakeLadderAlgoTime
            {
                RoundID = 1,
                AlgorithmName = "BFS",
                TimeTaken_ms = 1.0
            };
            var largeBoard = new SnakeLadderAlgoTime
            {
                RoundID = 2,
                AlgorithmName = "BFS",
                TimeTaken_ms = 50.0
            };

            // Act & Assert
            Assert.True(largeBoard.TimeTaken_ms > smallBoard.TimeTaken_ms);
        }

        [Fact]
        public void AlgorithmTime_ShouldSupportPerformanceAnalysis()
        {
            // Arrange
            var times = new List<SnakeLadderAlgoTime>
            {
                new SnakeLadderAlgoTime { AlgorithmName = "BFS", TimeTaken_ms = 2.5 },
                new SnakeLadderAlgoTime { AlgorithmName = "Dijkstra", TimeTaken_ms = 3.2 },
                new SnakeLadderAlgoTime { AlgorithmName = "Dynamic Programming", TimeTaken_ms = 1.8 }
            };

            // Act
            var fastest = times.OrderBy(t => t.TimeTaken_ms).First();
            var slowest = times.OrderByDescending(t => t.TimeTaken_ms).First();

            // Assert
            Assert.Equal("Dynamic Programming", fastest.AlgorithmName);
            Assert.Equal("Dijkstra", slowest.AlgorithmName);
        }

        [Fact]
        public void AlgorithmTime_ShouldTrackMillisecondPrecision()
        {
            // Arrange
            var algoTime1 = new SnakeLadderAlgoTime { TimeTaken_ms = 2.123 };
            var algoTime2 = new SnakeLadderAlgoTime { TimeTaken_ms = 2.124 };

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
            var round = new SnakeLadderRound { RoundID = 1, BoardSize_N = 100 };
            var benchmark1 = new SnakeLadderAlgoTime
            {
                RoundID = 1,
                AlgorithmName = "BFS V1",
                TimeTaken_ms = 5.0,
                Round = round
            };
            var benchmark2 = new SnakeLadderAlgoTime
            {
                RoundID = 1,
                AlgorithmName = "BFS V2",
                TimeTaken_ms = 4.5,
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
            var time1 = new SnakeLadderAlgoTime { RoundID = 1, AlgorithmName = "BFS", TimeTaken_ms = 2.0 };
            var time2 = new SnakeLadderAlgoTime { RoundID = 2, AlgorithmName = "BFS", TimeTaken_ms = 3.0 };
            var time3 = new SnakeLadderAlgoTime { RoundID = 3, AlgorithmName = "BFS", TimeTaken_ms = 4.0 };

            // Act
            var times = new List<SnakeLadderAlgoTime> { time1, time2, time3 };
            var averageTime = times.Average(t => t.TimeTaken_ms);

            // Assert
            Assert.Equal(3.0, averageTime);
            Assert.Equal(3, times.Select(t => t.RoundID).Distinct().Count());
        }

        [Fact]
        public void AlgorithmTime_ShouldSupportComplexityComparison()
        {
            // Arrange
            var linearTime = new SnakeLadderAlgoTime
            {
                AlgorithmName = "Linear Search",
                TimeTaken_ms = 1.0
            };
            var logTime = new SnakeLadderAlgoTime
            {
                AlgorithmName = "Binary Search",
                TimeTaken_ms = 0.1
            };

            // Act
            var timeDifference = linearTime.TimeTaken_ms / logTime.TimeTaken_ms;

            // Assert
            Assert.Equal(10.0, timeDifference);
        }

        #endregion
    }
}
