using PDSA.API.Data.Models;
using PDSA.API.Data.Models.TSP;
using System.ComponentModel.DataAnnotations;

namespace PDSA.Tests.Data.Models.TSP
{
    public class TSPRoundTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void TSPRound_Should_BeInstantiated()
        {
            // Act
            var round = new TSPRound();

            // Assert
            Assert.NotNull(round);
        }

        [Fact]
        public void TSPRound_Should_HaveDefaultValues()
        {
            // Act
            var round = new TSPRound();

            // Assert
            Assert.Equal(0, round.RoundID);
            Assert.Equal(0, round.PlayerID);
            Assert.Equal(string.Empty, round.HomeCity);
            Assert.Equal(string.Empty, round.SelectedCities);
            Assert.Equal(string.Empty, round.ShortestRoute_Path);
            Assert.Equal(0, round.ShortestRoute_Distance);
            Assert.Equal(string.Empty, round.DatePlayed);
            Assert.Null(round.Player);
            Assert.NotNull(round.Distances);
            Assert.Empty(round.Distances);
            Assert.NotNull(round.AlgorithmTimes);
            Assert.Empty(round.AlgorithmTimes);
        }

        [Fact]
        public void TSPRound_Should_SetAllProperties()
        {
            // Arrange
            var round = new TSPRound();
            var date = DateTime.UtcNow.ToString("o");

            // Act
            round.RoundID = 1;
            round.PlayerID = 42;
            round.HomeCity = "Delhi";
            round.SelectedCities = "Mumbai,Kolkata,Chennai";
            round.ShortestRoute_Path = "Delhi->Mumbai->Kolkata->Chennai->Delhi";
            round.ShortestRoute_Distance = 3500.5;
            round.DatePlayed = date;

            // Assert
            Assert.Equal(1, round.RoundID);
            Assert.Equal(42, round.PlayerID);
            Assert.Equal("Delhi", round.HomeCity);
            Assert.Equal("Mumbai,Kolkata,Chennai", round.SelectedCities);
            Assert.Equal("Delhi->Mumbai->Kolkata->Chennai->Delhi", round.ShortestRoute_Path);
            Assert.Equal(3500.5, round.ShortestRoute_Distance);
            Assert.Equal(date, round.DatePlayed);
        }

        [Fact]
        public void TSPRound_Should_AllowObjectInitializer()
        {
            // Act
            var round = new TSPRound
            {
                RoundID = 10,
                PlayerID = 5,
                HomeCity = "Bangalore",
                SelectedCities = "Hyderabad",
                ShortestRoute_Path = "Bangalore->Hyderabad->Bangalore",
                ShortestRoute_Distance = 1200.75,
                DatePlayed = "2025-01-01T10:00:00Z"
            };

            // Assert
            Assert.Equal(10, round.RoundID);
            Assert.Equal(5, round.PlayerID);
            Assert.Equal("Bangalore", round.HomeCity);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void TSPRound_RoundID_Should_HaveKeyAttribute()
        {
            // Arrange
            var property = typeof(TSPRound).GetProperty(nameof(TSPRound.RoundID));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TSPRound_PlayerID_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TSPRound).GetProperty(nameof(TSPRound.PlayerID));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TSPRound_HomeCity_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TSPRound).GetProperty(nameof(TSPRound.HomeCity));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TSPRound_Should_FailValidation_WhenRequiredFieldsEmpty()
        {
            // Arrange
            var round = new TSPRound();
            var validationContext = new ValidationContext(round);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(round, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(validationResults);
        }

        [Fact]
        public void TSPRound_Should_PassValidation_WhenAllFieldsProvided()
        {
            // Arrange
            var round = new TSPRound
            {
                PlayerID = 1,
                HomeCity = "Delhi",
                SelectedCities = "Mumbai",
                ShortestRoute_Path = "Delhi->Mumbai->Delhi",
                ShortestRoute_Distance = 1400,
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
        public void TSPRound_Should_SetPlayer()
        {
            // Arrange
            var round = new TSPRound();
            var player = new Player { PlayerID = 1, Name = "TestPlayer" };

            // Act
            round.Player = player;

            // Assert
            Assert.NotNull(round.Player);
            Assert.Equal("TestPlayer", round.Player.Name);
        }

        [Fact]
        public void TSPRound_Should_AddDistance()
        {
            // Arrange
            var round = new TSPRound();
            var distance = new TSPDistance
            {
                RoundID = round.RoundID,
                City_A = "Delhi",
                City_B = "Mumbai",
                Distance_km = 1400
            };

            // Act
            round.Distances.Add(distance);

            // Assert
            Assert.Single(round.Distances);
            Assert.Contains(distance, round.Distances);
        }

        [Fact]
        public void TSPRound_Should_AddMultipleDistances()
        {
            // Arrange
            var round = new TSPRound();
            var distance1 = new TSPDistance { City_A = "Delhi", City_B = "Mumbai", Distance_km = 1400 };
            var distance2 = new TSPDistance { City_A = "Delhi", City_B = "Kolkata", Distance_km = 1500 };
            var distance3 = new TSPDistance { City_A = "Mumbai", City_B = "Kolkata", Distance_km = 2000 };

            // Act
            round.Distances.Add(distance1);
            round.Distances.Add(distance2);
            round.Distances.Add(distance3);

            // Assert
            Assert.Equal(3, round.Distances.Count);
        }

        [Fact]
        public void TSPRound_Should_AddAlgorithmTime()
        {
            // Arrange
            var round = new TSPRound();
            var algoTime = new TSPAlgoTime
            {
                RoundID = round.RoundID,
                AlgorithmName = "Branch and Bound",
                TimeTaken_ms = 125.5
            };

            // Act
            round.AlgorithmTimes.Add(algoTime);

            // Assert
            Assert.Single(round.AlgorithmTimes);
            Assert.Contains(algoTime, round.AlgorithmTimes);
        }

        [Fact]
        public void TSPRound_Should_AddMultipleAlgorithmTimes()
        {
            // Arrange
            var round = new TSPRound();
            var algoTime1 = new TSPAlgoTime { AlgorithmName = "Branch and Bound", TimeTaken_ms = 100 };
            var algoTime2 = new TSPAlgoTime { AlgorithmName = "Nearest Neighbor", TimeTaken_ms = 50 };

            // Act
            round.AlgorithmTimes.Add(algoTime1);
            round.AlgorithmTimes.Add(algoTime2);

            // Assert
            Assert.Equal(2, round.AlgorithmTimes.Count);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void TSPRound_Should_AllowVeryLongCityNames()
        {
            // Arrange
            var round = new TSPRound();
            var longCityName = new string('A', 500);

            // Act
            round.HomeCity = longCityName;
            round.SelectedCities = longCityName;

            // Assert
            Assert.Equal(500, round.HomeCity.Length);
            Assert.Equal(500, round.SelectedCities.Length);
        }

        [Fact]
        public void TSPRound_Should_AllowZeroDistance()
        {
            // Arrange
            var round = new TSPRound();

            // Act
            round.ShortestRoute_Distance = 0;

            // Assert
            Assert.Equal(0, round.ShortestRoute_Distance);
        }

        [Fact]
        public void TSPRound_Should_AllowVeryLargeDistance()
        {
            // Arrange
            var round = new TSPRound();

            // Act
            round.ShortestRoute_Distance = 999999.99;

            // Assert
            Assert.Equal(999999.99, round.ShortestRoute_Distance);
        }

        [Fact]
        public void TSPRound_Should_AllowManyCities()
        {
            // Arrange
            var round = new TSPRound();
            var cities = string.Join(",", Enumerable.Range(1, 50).Select(i => $"City{i}"));

            // Act
            round.SelectedCities = cities;

            // Assert
            Assert.Contains("City1", round.SelectedCities);
            Assert.Contains("City50", round.SelectedCities);
        }

        #endregion
    }
}
