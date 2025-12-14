using PDSA.API.Data.Models.TrafficSimulation;
using System.ComponentModel.DataAnnotations;

namespace PDSA.Tests.Data.Models.TrafficSimulation
{
    public class TrafficAlgoTimeTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void TrafficAlgoTime_Should_BeInstantiated()
        {
            // Act
            var algoTime = new TrafficAlgoTime();

            // Assert
            Assert.NotNull(algoTime);
        }

        [Fact]
        public void TrafficAlgoTime_Should_HaveDefaultValues()
        {
            // Act
            var algoTime = new TrafficAlgoTime();

            // Assert
            Assert.Equal(0, algoTime.TimeID);
            Assert.Equal(0, algoTime.RoundID);
            Assert.Equal(string.Empty, algoTime.AlgorithmName);
            Assert.Equal(0, algoTime.TimeTaken_ms);
            Assert.Null(algoTime.Round);
        }

        [Fact]
        public void TrafficAlgoTime_Should_SetAllProperties()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime();

            // Act
            algoTime.TimeID = 1;
            algoTime.RoundID = 10;
            algoTime.AlgorithmName = "Edmonds-Karp";
            algoTime.TimeTaken_ms = 125.75;

            // Assert
            Assert.Equal(1, algoTime.TimeID);
            Assert.Equal(10, algoTime.RoundID);
            Assert.Equal("Edmonds-Karp", algoTime.AlgorithmName);
            Assert.Equal(125.75, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TrafficAlgoTime_Should_AllowObjectInitializer()
        {
            // Act
            var algoTime = new TrafficAlgoTime
            {
                TimeID = 5,
                RoundID = 3,
                AlgorithmName = "Dinic",
                TimeTaken_ms = 50.25
            };

            // Assert
            Assert.Equal(5, algoTime.TimeID);
            Assert.Equal(3, algoTime.RoundID);
            Assert.Equal("Dinic", algoTime.AlgorithmName);
            Assert.Equal(50.25, algoTime.TimeTaken_ms);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void TrafficAlgoTime_TimeID_Should_HaveKeyAttribute()
        {
            // Arrange
            var property = typeof(TrafficAlgoTime).GetProperty(nameof(TrafficAlgoTime.TimeID));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TrafficAlgoTime_RoundID_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TrafficAlgoTime).GetProperty(nameof(TrafficAlgoTime.RoundID));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TrafficAlgoTime_AlgorithmName_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TrafficAlgoTime).GetProperty(nameof(TrafficAlgoTime.AlgorithmName));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TrafficAlgoTime_TimeTaken_ms_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TrafficAlgoTime).GetProperty(nameof(TrafficAlgoTime.TimeTaken_ms));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TrafficAlgoTime_Should_FailValidation_WhenRequiredFieldsEmpty()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime();
            var validationContext = new ValidationContext(algoTime);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(algoTime, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(validationResults);
        }

        [Fact]
        public void TrafficAlgoTime_Should_PassValidation_WhenAllFieldsProvided()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime
            {
                RoundID = 1,
                AlgorithmName = "Edmonds-Karp",
                TimeTaken_ms = 100.5
            };
            var validationContext = new ValidationContext(algoTime);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(algoTime, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        #endregion

        #region Navigation Property Tests

        [Fact]
        public void TrafficAlgoTime_Should_SetRound()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime();
            var round = new TrafficRound
            {
                RoundID = 1,
                PlayerID = 1,
                CorrectMaxFlow = 20,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            // Act
            algoTime.Round = round;

            // Assert
            Assert.NotNull(algoTime.Round);
            Assert.Equal(20, algoTime.Round.CorrectMaxFlow);
        }

        #endregion

        #region Algorithm Name Tests

        [Fact]
        public void TrafficAlgoTime_Should_AcceptEdmondsKarpAlgorithm()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime();

            // Act
            algoTime.AlgorithmName = "Edmonds-Karp";

            // Assert
            Assert.Equal("Edmonds-Karp", algoTime.AlgorithmName);
        }

        [Fact]
        public void TrafficAlgoTime_Should_AcceptDinicAlgorithm()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime();

            // Act
            algoTime.AlgorithmName = "Dinic";

            // Assert
            Assert.Equal("Dinic", algoTime.AlgorithmName);
        }

        [Fact]
        public void TrafficAlgoTime_Should_AcceptFordFulkersonAlgorithm()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime();

            // Act
            algoTime.AlgorithmName = "Ford-Fulkerson";

            // Assert
            Assert.Equal("Ford-Fulkerson", algoTime.AlgorithmName);
        }

        [Fact]
        public void TrafficAlgoTime_Should_AcceptPushRelabelAlgorithm()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime();

            // Act
            algoTime.AlgorithmName = "Push-Relabel";

            // Assert
            Assert.Equal("Push-Relabel", algoTime.AlgorithmName);
        }

        [Fact]
        public void TrafficAlgoTime_Should_AcceptCustomAlgorithmName()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime();

            // Act
            algoTime.AlgorithmName = "Custom Max Flow Algorithm";

            // Assert
            Assert.Equal("Custom Max Flow Algorithm", algoTime.AlgorithmName);
        }

        #endregion

        #region Time Measurement Tests

        [Fact]
        public void TrafficAlgoTime_Should_AllowZeroTime()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 0;

            // Assert
            Assert.Equal(0, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TrafficAlgoTime_Should_AllowVerySmallTime()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 0.001;

            // Assert
            Assert.Equal(0.001, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TrafficAlgoTime_Should_AllowVeryLargeTime()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 9999999.999;

            // Assert
            Assert.Equal(9999999.999, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TrafficAlgoTime_Should_AcceptPreciseDecimalValues()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 123.456789;

            // Assert
            Assert.Equal(123.456789, algoTime.TimeTaken_ms);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void TrafficAlgoTime_Should_AllowLongAlgorithmName()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime();
            var longName = "Very Long Algorithm Name With Detailed Description " + new string('A', 200);

            // Act
            algoTime.AlgorithmName = longName;

            // Assert
            Assert.Equal(longName, algoTime.AlgorithmName);
        }

        [Fact]
        public void TrafficAlgoTime_Should_AllowAlgorithmNameWithSpecialCharacters()
        {
            // Arrange
            var algoTime = new TrafficAlgoTime();

            // Act
            algoTime.AlgorithmName = "Edmonds-Karp (Optimized) v2.0";

            // Assert
            Assert.Equal("Edmonds-Karp (Optimized) v2.0", algoTime.AlgorithmName);
        }

        [Fact]
        public void TrafficAlgoTime_Should_SupportMultipleAlgorithmsForSameRound()
        {
            // Arrange
            var algoTime1 = new TrafficAlgoTime { RoundID = 1, AlgorithmName = "Edmonds-Karp", TimeTaken_ms = 100 };
            var algoTime2 = new TrafficAlgoTime { RoundID = 1, AlgorithmName = "Dinic", TimeTaken_ms = 50 };

            // Assert
            Assert.Equal(algoTime1.RoundID, algoTime2.RoundID);
            Assert.NotEqual(algoTime1.AlgorithmName, algoTime2.AlgorithmName);
        }

        #endregion

        #region Comparison Tests

        [Fact]
        public void TrafficAlgoTime_Should_CompareTimes()
        {
            // Arrange
            var algoTime1 = new TrafficAlgoTime { TimeTaken_ms = 100.5 };
            var algoTime2 = new TrafficAlgoTime { TimeTaken_ms = 200.5 };

            // Assert
            Assert.True(algoTime1.TimeTaken_ms < algoTime2.TimeTaken_ms);
        }

        [Fact]
        public void TrafficAlgoTime_Should_IdentifyFasterAlgorithm()
        {
            // Arrange
            var edmondsKarp = new TrafficAlgoTime { AlgorithmName = "Edmonds-Karp", TimeTaken_ms = 150 };
            var dinic = new TrafficAlgoTime { AlgorithmName = "Dinic", TimeTaken_ms = 75 };

            // Assert
            Assert.True(dinic.TimeTaken_ms < edmondsKarp.TimeTaken_ms);
        }

        [Fact]
        public void TrafficAlgoTime_Should_DetermineSpeedupFactor()
        {
            // Arrange
            var slowAlgo = new TrafficAlgoTime { AlgorithmName = "Slow", TimeTaken_ms = 200 };
            var fastAlgo = new TrafficAlgoTime { AlgorithmName = "Fast", TimeTaken_ms = 50 };

            // Act
            var speedup = slowAlgo.TimeTaken_ms / fastAlgo.TimeTaken_ms;

            // Assert
            Assert.Equal(4, speedup);
        }

        #endregion
    }
}
