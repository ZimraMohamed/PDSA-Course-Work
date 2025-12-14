using PDSA.API.Data.Models;
using PDSA.API.Data.Models.TrafficSimulation;
using System.ComponentModel.DataAnnotations;

namespace PDSA.Tests.Data.Models.TrafficSimulation
{
    public class TrafficRoundTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void TrafficRound_Should_BeInstantiated()
        {
            // Act
            var round = new TrafficRound();

            // Assert
            Assert.NotNull(round);
        }

        [Fact]
        public void TrafficRound_Should_HaveDefaultValues()
        {
            // Act
            var round = new TrafficRound();

            // Assert
            Assert.Equal(0, round.RoundID);
            Assert.Equal(0, round.PlayerID);
            Assert.Equal(0, round.CorrectMaxFlow);
            Assert.Equal(string.Empty, round.DatePlayed);
            Assert.Null(round.Player);
            Assert.NotNull(round.Capacities);
            Assert.Empty(round.Capacities);
            Assert.NotNull(round.AlgorithmTimes);
            Assert.Empty(round.AlgorithmTimes);
        }

        [Fact]
        public void TrafficRound_Should_SetAllProperties()
        {
            // Arrange
            var round = new TrafficRound();
            var date = DateTime.UtcNow.ToString("o");

            // Act
            round.RoundID = 1;
            round.PlayerID = 42;
            round.CorrectMaxFlow = 25.5;
            round.DatePlayed = date;

            // Assert
            Assert.Equal(1, round.RoundID);
            Assert.Equal(42, round.PlayerID);
            Assert.Equal(25.5, round.CorrectMaxFlow);
            Assert.Equal(date, round.DatePlayed);
        }

        [Fact]
        public void TrafficRound_Should_AllowObjectInitializer()
        {
            // Act
            var round = new TrafficRound
            {
                RoundID = 10,
                PlayerID = 5,
                CorrectMaxFlow = 30,
                DatePlayed = "2025-01-01T10:00:00Z"
            };

            // Assert
            Assert.Equal(10, round.RoundID);
            Assert.Equal(5, round.PlayerID);
            Assert.Equal(30, round.CorrectMaxFlow);
            Assert.Equal("2025-01-01T10:00:00Z", round.DatePlayed);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void TrafficRound_RoundID_Should_HaveKeyAttribute()
        {
            // Arrange
            var property = typeof(TrafficRound).GetProperty(nameof(TrafficRound.RoundID));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TrafficRound_PlayerID_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TrafficRound).GetProperty(nameof(TrafficRound.PlayerID));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TrafficRound_CorrectMaxFlow_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TrafficRound).GetProperty(nameof(TrafficRound.CorrectMaxFlow));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TrafficRound_DatePlayed_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TrafficRound).GetProperty(nameof(TrafficRound.DatePlayed));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TrafficRound_Should_FailValidation_WhenRequiredFieldsEmpty()
        {
            // Arrange
            var round = new TrafficRound();
            var validationContext = new ValidationContext(round);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(round, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(validationResults);
        }

        [Fact]
        public void TrafficRound_Should_PassValidation_WhenAllFieldsProvided()
        {
            // Arrange
            var round = new TrafficRound
            {
                PlayerID = 1,
                CorrectMaxFlow = 20,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };
            var validationContext = new ValidationContext(round);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(round, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        #endregion

        #region Navigation Property Tests

        [Fact]
        public void TrafficRound_Should_SetPlayer()
        {
            // Arrange
            var round = new TrafficRound();
            var player = new Player { PlayerID = 1, Name = "TestPlayer" };

            // Act
            round.Player = player;

            // Assert
            Assert.NotNull(round.Player);
            Assert.Equal("TestPlayer", round.Player.Name);
        }

        [Fact]
        public void TrafficRound_Should_AddCapacity()
        {
            // Arrange
            var round = new TrafficRound();
            var capacity = new TrafficCapacity
            {
                RoundID = round.RoundID,
                RoadSegment = "A-B",
                Capacity_VehPerMin = 10
            };

            // Act
            round.Capacities.Add(capacity);

            // Assert
            Assert.Single(round.Capacities);
            Assert.Contains(capacity, round.Capacities);
        }

        [Fact]
        public void TrafficRound_Should_AddMultipleCapacities()
        {
            // Arrange
            var round = new TrafficRound();
            var capacity1 = new TrafficCapacity { RoadSegment = "A-B", Capacity_VehPerMin = 10 };
            var capacity2 = new TrafficCapacity { RoadSegment = "B-C", Capacity_VehPerMin = 15 };
            var capacity3 = new TrafficCapacity { RoadSegment = "C-D", Capacity_VehPerMin = 20 };

            // Act
            round.Capacities.Add(capacity1);
            round.Capacities.Add(capacity2);
            round.Capacities.Add(capacity3);

            // Assert
            Assert.Equal(3, round.Capacities.Count);
        }

        [Fact]
        public void TrafficRound_Should_AddAlgorithmTime()
        {
            // Arrange
            var round = new TrafficRound();
            var algoTime = new TrafficAlgoTime
            {
                RoundID = round.RoundID,
                AlgorithmName = "Edmonds-Karp",
                TimeTaken_ms = 75.5
            };

            // Act
            round.AlgorithmTimes.Add(algoTime);

            // Assert
            Assert.Single(round.AlgorithmTimes);
            Assert.Contains(algoTime, round.AlgorithmTimes);
        }

        [Fact]
        public void TrafficRound_Should_AddMultipleAlgorithmTimes()
        {
            // Arrange
            var round = new TrafficRound();
            var algoTime1 = new TrafficAlgoTime { AlgorithmName = "Edmonds-Karp", TimeTaken_ms = 100 };
            var algoTime2 = new TrafficAlgoTime { AlgorithmName = "Dinic", TimeTaken_ms = 50 };

            // Act
            round.AlgorithmTimes.Add(algoTime1);
            round.AlgorithmTimes.Add(algoTime2);

            // Assert
            Assert.Equal(2, round.AlgorithmTimes.Count);
        }

        #endregion

        #region Max Flow Tests

        [Fact]
        public void TrafficRound_Should_AllowZeroMaxFlow()
        {
            // Arrange
            var round = new TrafficRound();

            // Act
            round.CorrectMaxFlow = 0;

            // Assert
            Assert.Equal(0, round.CorrectMaxFlow);
        }

        [Fact]
        public void TrafficRound_Should_AllowDecimalMaxFlow()
        {
            // Arrange
            var round = new TrafficRound();

            // Act
            round.CorrectMaxFlow = 25.75;

            // Assert
            Assert.Equal(25.75, round.CorrectMaxFlow);
        }

        [Fact]
        public void TrafficRound_Should_AllowVeryLargeMaxFlow()
        {
            // Arrange
            var round = new TrafficRound();

            // Act
            round.CorrectMaxFlow = 999999.99;

            // Assert
            Assert.Equal(999999.99, round.CorrectMaxFlow);
        }

        [Fact]
        public void TrafficRound_Should_AllowPreciseDecimalValues()
        {
            // Arrange
            var round = new TrafficRound();

            // Act
            round.CorrectMaxFlow = 123.456789;

            // Assert
            Assert.Equal(123.456789, round.CorrectMaxFlow);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void TrafficRound_Should_RemoveCapacity()
        {
            // Arrange
            var round = new TrafficRound();
            var capacity = new TrafficCapacity { RoadSegment = "A-B", Capacity_VehPerMin = 10 };
            round.Capacities.Add(capacity);

            // Act
            round.Capacities.Remove(capacity);

            // Assert
            Assert.Empty(round.Capacities);
        }

        [Fact]
        public void TrafficRound_Should_ClearAllCapacities()
        {
            // Arrange
            var round = new TrafficRound();
            round.Capacities.Add(new TrafficCapacity { RoadSegment = "A-B", Capacity_VehPerMin = 10 });
            round.Capacities.Add(new TrafficCapacity { RoadSegment = "B-C", Capacity_VehPerMin = 15 });

            // Act
            round.Capacities.Clear();

            // Assert
            Assert.Empty(round.Capacities);
        }

        #endregion
    }
}
