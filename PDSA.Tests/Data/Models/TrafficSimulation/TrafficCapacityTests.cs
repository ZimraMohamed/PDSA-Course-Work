using PDSA.API.Data.Models.TrafficSimulation;
using System.ComponentModel.DataAnnotations;

namespace PDSA.Tests.Data.Models.TrafficSimulation
{
    public class TrafficCapacityTests
    {
        #region Constructor and Property Tests

        [Fact]
        public void TrafficCapacity_Should_BeInstantiated()
        {
            // Act
            var capacity = new TrafficCapacity();

            // Assert
            Assert.NotNull(capacity);
        }

        [Fact]
        public void TrafficCapacity_Should_HaveDefaultValues()
        {
            // Act
            var capacity = new TrafficCapacity();

            // Assert
            Assert.Equal(0, capacity.CapacityID);
            Assert.Equal(0, capacity.RoundID);
            Assert.Equal(string.Empty, capacity.RoadSegment);
            Assert.Equal(0, capacity.Capacity_VehPerMin);
            Assert.Null(capacity.Round);
        }

        [Fact]
        public void TrafficCapacity_Should_SetAllProperties()
        {
            // Arrange
            var capacity = new TrafficCapacity();

            // Act
            capacity.CapacityID = 1;
            capacity.RoundID = 10;
            capacity.RoadSegment = "A-B";
            capacity.Capacity_VehPerMin = 15;

            // Assert
            Assert.Equal(1, capacity.CapacityID);
            Assert.Equal(10, capacity.RoundID);
            Assert.Equal("A-B", capacity.RoadSegment);
            Assert.Equal(15, capacity.Capacity_VehPerMin);
        }

        [Fact]
        public void TrafficCapacity_Should_AllowObjectInitializer()
        {
            // Act
            var capacity = new TrafficCapacity
            {
                CapacityID = 5,
                RoundID = 3,
                RoadSegment = "B-C",
                Capacity_VehPerMin = 20
            };

            // Assert
            Assert.Equal(5, capacity.CapacityID);
            Assert.Equal(3, capacity.RoundID);
            Assert.Equal("B-C", capacity.RoadSegment);
            Assert.Equal(20, capacity.Capacity_VehPerMin);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void TrafficCapacity_CapacityID_Should_HaveKeyAttribute()
        {
            // Arrange
            var property = typeof(TrafficCapacity).GetProperty(nameof(TrafficCapacity.CapacityID));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TrafficCapacity_RoundID_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TrafficCapacity).GetProperty(nameof(TrafficCapacity.RoundID));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TrafficCapacity_RoadSegment_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TrafficCapacity).GetProperty(nameof(TrafficCapacity.RoadSegment));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TrafficCapacity_Capacity_VehPerMin_Should_HaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TrafficCapacity).GetProperty(nameof(TrafficCapacity.Capacity_VehPerMin));

            // Act
            var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void TrafficCapacity_Should_FailValidation_WhenRequiredFieldsEmpty()
        {
            // Arrange
            var capacity = new TrafficCapacity();
            var validationContext = new ValidationContext(capacity);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(capacity, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(validationResults);
        }

        [Fact]
        public void TrafficCapacity_Should_PassValidation_WhenAllFieldsProvided()
        {
            // Arrange
            var capacity = new TrafficCapacity
            {
                RoundID = 1,
                RoadSegment = "A-B",
                Capacity_VehPerMin = 10
            };
            var validationContext = new ValidationContext(capacity);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(capacity, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        #endregion

        #region Navigation Property Tests

        [Fact]
        public void TrafficCapacity_Should_SetRound()
        {
            // Arrange
            var capacity = new TrafficCapacity();
            var round = new TrafficRound
            {
                RoundID = 1,
                PlayerID = 1,
                CorrectMaxFlow = 20,
                DatePlayed = DateTime.UtcNow.ToString("o")
            };

            // Act
            capacity.Round = round;

            // Assert
            Assert.NotNull(capacity.Round);
            Assert.Equal(20, capacity.Round.CorrectMaxFlow);
        }

        #endregion

        #region Road Segment Tests

        [Fact]
        public void TrafficCapacity_Should_AcceptStandardRoadSegmentFormat()
        {
            // Arrange
            var capacity = new TrafficCapacity();

            // Act
            capacity.RoadSegment = "A-B";

            // Assert
            Assert.Equal("A-B", capacity.RoadSegment);
        }

        [Fact]
        public void TrafficCapacity_Should_AcceptLongRoadSegmentNames()
        {
            // Arrange
            var capacity = new TrafficCapacity();

            // Act
            capacity.RoadSegment = "Source-Destination";

            // Assert
            Assert.Equal("Source-Destination", capacity.RoadSegment);
        }

        [Fact]
        public void TrafficCapacity_Should_AcceptNumericRoadSegments()
        {
            // Arrange
            var capacity = new TrafficCapacity();

            // Act
            capacity.RoadSegment = "0-1";

            // Assert
            Assert.Equal("0-1", capacity.RoadSegment);
        }

        [Fact]
        public void TrafficCapacity_Should_AcceptRoadSegmentWithUnderscore()
        {
            // Arrange
            var capacity = new TrafficCapacity();

            // Act
            capacity.RoadSegment = "Node_A-Node_B";

            // Assert
            Assert.Equal("Node_A-Node_B", capacity.RoadSegment);
        }

        #endregion

        #region Capacity Tests

        [Fact]
        public void TrafficCapacity_Should_AllowZeroCapacity()
        {
            // Arrange
            var capacity = new TrafficCapacity();

            // Act
            capacity.Capacity_VehPerMin = 0;

            // Assert
            Assert.Equal(0, capacity.Capacity_VehPerMin);
        }

        [Fact]
        public void TrafficCapacity_Should_AllowSmallCapacity()
        {
            // Arrange
            var capacity = new TrafficCapacity();

            // Act
            capacity.Capacity_VehPerMin = 1;

            // Assert
            Assert.Equal(1, capacity.Capacity_VehPerMin);
        }

        [Fact]
        public void TrafficCapacity_Should_AllowLargeCapacity()
        {
            // Arrange
            var capacity = new TrafficCapacity();

            // Act
            capacity.Capacity_VehPerMin = 10000;

            // Assert
            Assert.Equal(10000, capacity.Capacity_VehPerMin);
        }

        [Fact]
        public void TrafficCapacity_Should_AllowMaxIntCapacity()
        {
            // Arrange
            var capacity = new TrafficCapacity();

            // Act
            capacity.Capacity_VehPerMin = int.MaxValue;

            // Assert
            Assert.Equal(int.MaxValue, capacity.Capacity_VehPerMin);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void TrafficCapacity_Should_AllowReverseDirectionSegment()
        {
            // Arrange
            var capacity1 = new TrafficCapacity { RoadSegment = "A-B", Capacity_VehPerMin = 10 };
            var capacity2 = new TrafficCapacity { RoadSegment = "B-A", Capacity_VehPerMin = 15 };

            // Assert
            Assert.NotEqual(capacity1.RoadSegment, capacity2.RoadSegment);
        }

        [Fact]
        public void TrafficCapacity_Should_AllowSpecialCharactersInSegment()
        {
            // Arrange
            var capacity = new TrafficCapacity();

            // Act
            capacity.RoadSegment = "Node(A)->Node(B)";

            // Assert
            Assert.Equal("Node(A)->Node(B)", capacity.RoadSegment);
        }

        [Fact]
        public void TrafficCapacity_Should_SupportMultipleCapacitiesForSameRound()
        {
            // Arrange
            var capacity1 = new TrafficCapacity { RoundID = 1, RoadSegment = "A-B", Capacity_VehPerMin = 10 };
            var capacity2 = new TrafficCapacity { RoundID = 1, RoadSegment = "B-C", Capacity_VehPerMin = 20 };

            // Assert
            Assert.Equal(capacity1.RoundID, capacity2.RoundID);
            Assert.NotEqual(capacity1.RoadSegment, capacity2.RoadSegment);
        }

        #endregion
    }
}
