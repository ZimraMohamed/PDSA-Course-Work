using PDSA.API.Data.Models.TSP;
using System.ComponentModel.DataAnnotations;

namespace PDSA.Tests.Data.Models.TSP
{
    public class TSPAlgoTimeTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void TSPAlgoTime_Should_BeInstantiated()
        {
            // Act
            var algoTime = new TSPAlgoTime();

            // Assert
            Assert.NotNull(algoTime);
        }

        [Fact]
        public void TSPAlgoTime_Should_HaveDefaultValues()
        {
            // Act
            var algoTime = new TSPAlgoTime();

            // Assert
            Assert.Equal(0, algoTime.TimeID);
            Assert.Equal(0, algoTime.RoundID);
            Assert.Equal(string.Empty, algoTime.AlgorithmName);
            Assert.Equal(0, algoTime.TimeTaken_ms);
            Assert.Null(algoTime.Round);
        }

        [Fact]
        public void TSPAlgoTime_Should_SetAllProperties()
        {
            // Arrange
            var algoTime = new TSPAlgoTime();

            // Act
            algoTime.TimeID = 1;
            algoTime.RoundID = 10;
            algoTime.AlgorithmName = "Branch and Bound";
            algoTime.TimeTaken_ms = 125.75;

            // Assert
            Assert.Equal(1, algoTime.TimeID);
            Assert.Equal(10, algoTime.RoundID);
            Assert.Equal("Branch and Bound", algoTime.AlgorithmName);
            Assert.Equal(125.75, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TSPAlgoTime_Should_AllowObjectInitializer()
        {
            // Act
            var algoTime = new TSPAlgoTime
            {
                TimeID = 5,
                RoundID = 3,
                AlgorithmName = "Nearest Neighbor",
                TimeTaken_ms = 50.25
            };

            // Assert
            Assert.Equal(5, algoTime.TimeID);
            Assert.Equal(3, algoTime.RoundID);
            Assert.Equal("Nearest Neighbor", algoTime.AlgorithmName);
            Assert.Equal(50.25, algoTime.TimeTaken_ms);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void TSPAlgoTime_TimeID_Should_HaveKeyAttribute()
        {
            // Arrange
            var property = typeof(TSPAlgoTime).GetProperty(nameof(TSPAlgoTime.TimeID));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TSPAlgoTime_RoundID_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TSPAlgoTime).GetProperty(nameof(TSPAlgoTime.RoundID));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TSPAlgoTime_AlgorithmName_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TSPAlgoTime).GetProperty(nameof(TSPAlgoTime.AlgorithmName));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TSPAlgoTime_TimeTaken_ms_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TSPAlgoTime).GetProperty(nameof(TSPAlgoTime.TimeTaken_ms));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TSPAlgoTime_Should_FailValidation_WhenRequiredFieldsEmpty()
        {
            // Arrange
            var algoTime = new TSPAlgoTime();
            var validationContext = new ValidationContext(algoTime);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(algoTime, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(validationResults);
        }

        [Fact]
        public void TSPAlgoTime_Should_PassValidation_WhenAllFieldsProvided()
        {
            // Arrange
            var algoTime = new TSPAlgoTime
            {
                RoundID = 1,
                AlgorithmName = "Branch and Bound",
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
        public void TSPAlgoTime_Should_SetRound()
        {
            // Arrange
            var algoTime = new TSPAlgoTime();
            var round = new TSPRound
            {
                RoundID = 1,
                HomeCity = "Delhi",
                SelectedCities = "Mumbai",
                ShortestRoute_Path = "Delhi->Mumbai->Delhi",
                ShortestRoute_Distance = 1400,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            // Act
            algoTime.Round = round;

            // Assert
            Assert.NotNull(algoTime.Round);
            Assert.Equal("Delhi", algoTime.Round.HomeCity);
        }

        #endregion

        #region Algorithm Name Tests

        [Fact]
        public void TSPAlgoTime_Should_AcceptBranchAndBoundAlgorithm()
        {
            // Arrange
            var algoTime = new TSPAlgoTime();

            // Act
            algoTime.AlgorithmName = "Branch and Bound";

            // Assert
            Assert.Equal("Branch and Bound", algoTime.AlgorithmName);
        }

        [Fact]
        public void TSPAlgoTime_Should_AcceptNearestNeighborAlgorithm()
        {
            // Arrange
            var algoTime = new TSPAlgoTime();

            // Act
            algoTime.AlgorithmName = "Nearest Neighbor";

            // Assert
            Assert.Equal("Nearest Neighbor", algoTime.AlgorithmName);
        }

        [Fact]
        public void TSPAlgoTime_Should_AcceptCustomAlgorithmName()
        {
            // Arrange
            var algoTime = new TSPAlgoTime();

            // Act
            algoTime.AlgorithmName = "Dynamic Programming";

            // Assert
            Assert.Equal("Dynamic Programming", algoTime.AlgorithmName);
        }

        #endregion

        #region Time Measurement Tests

        [Fact]
        public void TSPAlgoTime_Should_AllowZeroTime()
        {
            // Arrange
            var algoTime = new TSPAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 0;

            // Assert
            Assert.Equal(0, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TSPAlgoTime_Should_AllowVerySmallTime()
        {
            // Arrange
            var algoTime = new TSPAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 0.001;

            // Assert
            Assert.Equal(0.001, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TSPAlgoTime_Should_AllowVeryLargeTime()
        {
            // Arrange
            var algoTime = new TSPAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 9999999.999;

            // Assert
            Assert.Equal(9999999.999, algoTime.TimeTaken_ms);
        }

        [Fact]
        public void TSPAlgoTime_Should_AcceptPreciseDecimalValues()
        {
            // Arrange
            var algoTime = new TSPAlgoTime();

            // Act
            algoTime.TimeTaken_ms = 123.456789;

            // Assert
            Assert.Equal(123.456789, algoTime.TimeTaken_ms);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void TSPAlgoTime_Should_AllowLongAlgorithmName()
        {
            // Arrange
            var algoTime = new TSPAlgoTime();
            var longName = "Very Long Algorithm Name With Many Words " + new string('A', 200);

            // Act
            algoTime.AlgorithmName = longName;

            // Assert
            Assert.Equal(longName, algoTime.AlgorithmName);
        }

        [Fact]
        public void TSPAlgoTime_Should_AllowAlgorithmNameWithSpecialCharacters()
        {
            // Arrange
            var algoTime = new TSPAlgoTime();

            // Act
            algoTime.AlgorithmName = "Branch & Bound (Optimized) v2.0";

            // Assert
            Assert.Equal("Branch & Bound (Optimized) v2.0", algoTime.AlgorithmName);
        }

        #endregion

        #region Comparison Tests

        [Fact]
        public void TSPAlgoTime_Should_CompareTimes()
        {
            // Arrange
            var algoTime1 = new TSPAlgoTime { TimeTaken_ms = 100.5 };
            var algoTime2 = new TSPAlgoTime { TimeTaken_ms = 200.5 };

            // Assert
            Assert.True(algoTime1.TimeTaken_ms < algoTime2.TimeTaken_ms);
        }

        [Fact]
        public void TSPAlgoTime_Should_IdentifyFasterAlgorithm()
        {
            // Arrange
            var branchAndBound = new TSPAlgoTime { AlgorithmName = "Branch and Bound", TimeTaken_ms = 150 };
            var nearestNeighbor = new TSPAlgoTime { AlgorithmName = "Nearest Neighbor", TimeTaken_ms = 50 };

            // Assert
            Assert.True(nearestNeighbor.TimeTaken_ms < branchAndBound.TimeTaken_ms);
        }

        #endregion
    }
}
