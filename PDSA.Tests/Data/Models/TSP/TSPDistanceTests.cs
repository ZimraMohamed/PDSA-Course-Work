using PDSA.API.Data.Models.TSP;
using System.ComponentModel.DataAnnotations;

namespace PDSA.Tests.Data.Models.TSP
{
    public class TSPDistanceTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void TSPDistance_Should_BeInstantiated()
        {
            // Act
            var distance = new TSPDistance();

            // Assert
            Assert.NotNull(distance);
        }

        [Fact]
        public void TSPDistance_Should_HaveDefaultValues()
        {
            // Act
            var distance = new TSPDistance();

            // Assert
            Assert.Equal(0, distance.DistanceID);
            Assert.Equal(0, distance.RoundID);
            Assert.Equal(string.Empty, distance.City_A);
            Assert.Equal(string.Empty, distance.City_B);
            Assert.Equal(0, distance.Distance_km);
            Assert.Null(distance.Round);
        }

        [Fact]
        public void TSPDistance_Should_SetAllProperties()
        {
            // Arrange
            var distance = new TSPDistance();

            // Act
            distance.DistanceID = 1;
            distance.RoundID = 10;
            distance.City_A = "Delhi";
            distance.City_B = "Mumbai";
            distance.Distance_km = 1400;

            // Assert
            Assert.Equal(1, distance.DistanceID);
            Assert.Equal(10, distance.RoundID);
            Assert.Equal("Delhi", distance.City_A);
            Assert.Equal("Mumbai", distance.City_B);
            Assert.Equal(1400, distance.Distance_km);
        }

        [Fact]
        public void TSPDistance_Should_AllowObjectInitializer()
        {
            // Act
            var distance = new TSPDistance
            {
                DistanceID = 5,
                RoundID = 3,
                City_A = "Kolkata",
                City_B = "Chennai",
                Distance_km = 1650
            };

            // Assert
            Assert.Equal(5, distance.DistanceID);
            Assert.Equal(3, distance.RoundID);
            Assert.Equal("Kolkata", distance.City_A);
            Assert.Equal("Chennai", distance.City_B);
            Assert.Equal(1650, distance.Distance_km);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void TSPDistance_DistanceID_Should_HaveKeyAttribute()
        {
            // Arrange
            var property = typeof(TSPDistance).GetProperty(nameof(TSPDistance.DistanceID));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TSPDistance_RoundID_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TSPDistance).GetProperty(nameof(TSPDistance.RoundID));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TSPDistance_City_A_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TSPDistance).GetProperty(nameof(TSPDistance.City_A));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TSPDistance_City_B_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TSPDistance).GetProperty(nameof(TSPDistance.City_B));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TSPDistance_Distance_km_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TSPDistance).GetProperty(nameof(TSPDistance.Distance_km));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TSPDistance_Should_FailValidation_WhenRequiredFieldsEmpty()
        {
            // Arrange
            var distance = new TSPDistance();
            var validationContext = new ValidationContext(distance);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(distance, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(validationResults);
        }

        [Fact]
        public void TSPDistance_Should_PassValidation_WhenAllFieldsProvided()
        {
            // Arrange
            var distance = new TSPDistance
            {
                RoundID = 1,
                City_A = "Delhi",
                City_B = "Mumbai",
                Distance_km = 1400
            };
            var validationContext = new ValidationContext(distance);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(distance, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        #endregion

        #region Navigation Property Tests

        [Fact]
        public void TSPDistance_Should_SetRound()
        {
            // Arrange
            var distance = new TSPDistance();
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
            distance.Round = round;

            // Assert
            Assert.NotNull(distance.Round);
            Assert.Equal("Delhi", distance.Round.HomeCity);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void TSPDistance_Should_AllowSameCityNames()
        {
            // Arrange
            var distance = new TSPDistance();

            // Act
            distance.City_A = "TestCity";
            distance.City_B = "TestCity";

            // Assert
            Assert.Equal(distance.City_A, distance.City_B);
        }

        [Fact]
        public void TSPDistance_Should_AllowZeroDistance()
        {
            // Arrange
            var distance = new TSPDistance();

            // Act
            distance.Distance_km = 0;

            // Assert
            Assert.Equal(0, distance.Distance_km);
        }

        [Fact]
        public void TSPDistance_Should_AllowVeryLargeDistance()
        {
            // Arrange
            var distance = new TSPDistance();

            // Act
            distance.Distance_km = int.MaxValue;

            // Assert
            Assert.Equal(int.MaxValue, distance.Distance_km);
        }

        [Fact]
        public void TSPDistance_Should_AllowUnicodeInCityNames()
        {
            // Arrange
            var distance = new TSPDistance();

            // Act
            distance.City_A = "北京"; // Beijing in Chinese
            distance.City_B = "मुंबई"; // Mumbai in Hindi

            // Assert
            Assert.Equal("北京", distance.City_A);
            Assert.Equal("मुंबई", distance.City_B);
        }

        [Fact]
        public void TSPDistance_Should_AllowSpecialCharactersInCityNames()
        {
            // Arrange
            var distance = new TSPDistance();

            // Act
            distance.City_A = "San José";
            distance.City_B = "Saint-Pierre-et-Miquelon";

            // Assert
            Assert.Equal("San José", distance.City_A);
            Assert.Equal("Saint-Pierre-et-Miquelon", distance.City_B);
        }

        #endregion
    }
}
