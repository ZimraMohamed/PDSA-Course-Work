using PDSA.Core.Algorithms.TrafficSimulation;

namespace PDSA.Tests.Algorithms.TrafficSimulation
{
    public class TrafficGameServiceTests
    {
        [Fact]
        public void CalculateMaxFlow_Should_ReturnCorrectStatusForCorrectAnswer()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "B", Capacity = 10 },
                    new() { From = "B", To = "T", Capacity = 10 }
                }
            };
            var service = new TrafficGameService();
            int playerAnswer = 10;

            // Act
            var result = service.CalculateMaxFlow(network, playerAnswer);

            // Assert
            Assert.Equal(10, result.CorrectAnswer);
            Assert.Equal(10, result.PlayerAnswer);
            Assert.Equal("Correct", result.Status);
        }

        [Fact]
        public void CalculateMaxFlow_Should_ReturnWrongStatusForIncorrectAnswer()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "B", Capacity = 10 },
                    new() { From = "B", To = "T", Capacity = 10 }
                }
            };
            var service = new TrafficGameService();
            int playerAnswer = 15; // Wrong answer

            // Act
            var result = service.CalculateMaxFlow(network, playerAnswer);

            // Assert
            Assert.Equal(10, result.CorrectAnswer);
            Assert.Equal(15, result.PlayerAnswer);
            Assert.Equal("Wrong", result.Status);
        }

        [Fact]
        public void CalculateMaxFlow_Should_ExecuteBothAlgorithms()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "B", Capacity = 20 },
                    new() { From = "B", To = "T", Capacity = 15 }
                }
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, 15);

            // Assert - Both algorithms should be executed and timed
            Assert.True(result.EdmondsKarpTime >= 0);
            Assert.True(result.DinicTime >= 0);
        }

        [Fact]
        public void CalculateMaxFlow_Should_SetLegacyTimeTakenToEdmondsKarpTime()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "B", Capacity = 10 },
                    new() { From = "B", To = "T", Capacity = 10 }
                }
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, 10);

            // Assert - TimeTaken should match EdmondsKarpTime for backward compatibility
            Assert.Equal(result.EdmondsKarpTime, result.TimeTaken);
        }

        [Fact]
        public void CalculateMaxFlow_Should_HandleComplexNetwork()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "B", Capacity = 10 },
                    new() { From = "A", To = "C", Capacity = 10 },
                    new() { From = "B", To = "D", Capacity = 5 },
                    new() { From = "C", To = "D", Capacity = 10 },
                    new() { From = "D", To = "T", Capacity = 15 }
                }
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, 15);

            // Assert
            Assert.Equal(15, result.CorrectAnswer);
            Assert.Equal("Correct", result.Status);
        }

        [Fact]
        public void CalculateMaxFlow_Should_HandleBottleneckNetwork()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "B", Capacity = 100 },
                    new() { From = "B", To = "C", Capacity = 1 }, // Bottleneck
                    new() { From = "C", To = "T", Capacity = 100 }
                }
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, 1);

            // Assert
            Assert.Equal(1, result.CorrectAnswer);
            Assert.Equal("Correct", result.Status);
        }

        [Fact]
        public void CalculateMaxFlow_Should_HandleMultiplePathsNetwork()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "B", Capacity = 10 },
                    new() { From = "A", To = "C", Capacity = 5 },
                    new() { From = "B", To = "T", Capacity = 10 },
                    new() { From = "C", To = "T", Capacity = 5 }
                }
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, 15);

            // Assert
            Assert.Equal(15, result.CorrectAnswer);
            Assert.Equal("Correct", result.Status);
        }

        [Fact]
        public void CalculateMaxFlow_Should_HandleNoPathNetwork()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "B", Capacity = 10 },
                    new() { From = "C", To = "T", Capacity = 10 }
                    // No path from A to T
                }
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, 0);

            // Assert
            Assert.Equal(0, result.CorrectAnswer);
            Assert.Equal("Correct", result.Status);
        }

        [Fact]
        public void CalculateMaxFlow_Should_HandleEmptyNetwork()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>()
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, 0);

            // Assert
            Assert.Equal(0, result.CorrectAnswer);
            Assert.Equal("Correct", result.Status);
        }

        [Fact]
        public void CalculateMaxFlow_Should_AcceptZeroAsCorrectAnswerWhenNoFlow()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "B", Capacity = 0 }
                }
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, 0);

            // Assert
            Assert.Equal(0, result.CorrectAnswer);
            Assert.Equal("Correct", result.Status);
        }

        [Fact]
        public void CalculateMaxFlow_Should_HandlePlayerAnswerZeroWhenFlowExists()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "B", Capacity = 10 },
                    new() { From = "B", To = "T", Capacity = 10 }
                }
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, 0);

            // Assert
            Assert.Equal(10, result.CorrectAnswer);
            Assert.Equal(0, result.PlayerAnswer);
            Assert.Equal("Wrong", result.Status);
        }

        [Fact]
        public void CalculateMaxFlow_Should_HandleNegativePlayerAnswer()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "T", Capacity = 10 }
                }
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, -5);

            // Assert
            Assert.Equal(10, result.CorrectAnswer);
            Assert.Equal(-5, result.PlayerAnswer);
            Assert.Equal("Wrong", result.Status);
        }

        [Fact]
        public void CalculateMaxFlow_Should_HandleExcessivePlayerAnswer()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "T", Capacity = 5 }
                }
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, 1000);

            // Assert
            Assert.Equal(5, result.CorrectAnswer);
            Assert.Equal(1000, result.PlayerAnswer);
            Assert.Equal("Wrong", result.Status);
        }

        [Fact]
        public void CalculateMaxFlow_Should_HandleSingleEdgeNetwork()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "T", Capacity = 25 }
                }
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, 25);

            // Assert
            Assert.Equal(25, result.CorrectAnswer);
            Assert.Equal("Correct", result.Status);
        }

        [Fact]
        public void CalculateMaxFlow_Should_HandleNetworkWithCycles()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "B", Capacity = 10 },
                    new() { From = "B", To = "C", Capacity = 10 },
                    new() { From = "C", To = "A", Capacity = 10 }, // Cycle
                    new() { From = "C", To = "T", Capacity = 5 }
                }
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, 5);

            // Assert
            Assert.Equal(5, result.CorrectAnswer);
            Assert.Equal("Correct", result.Status);
        }

        [Fact]
        public void CalculateMaxFlow_Should_HandleLargeCapacityNetwork()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "B", Capacity = 1000000 },
                    new() { From = "B", To = "T", Capacity = 999999 }
                }
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, 999999);

            // Assert
            Assert.Equal(999999, result.CorrectAnswer);
            Assert.Equal("Correct", result.Status);
        }

        [Fact]
        public void TrafficEdge_Should_StoreProperties()
        {
            // Arrange & Act
            var edge = new TrafficEdge
            {
                From = "A",
                To = "B",
                Capacity = 50
            };

            // Assert
            Assert.Equal("A", edge.From);
            Assert.Equal("B", edge.To);
            Assert.Equal(50, edge.Capacity);
        }

        [Fact]
        public void TrafficNetwork_Should_InitializeWithEmptyEdgesList()
        {
            // Arrange & Act
            var network = new TrafficNetwork();

            // Assert
            Assert.NotNull(network.Edges);
            Assert.Empty(network.Edges);
        }

        [Fact]
        public void TrafficNetwork_Should_AllowAddingEdges()
        {
            // Arrange
            var network = new TrafficNetwork();

            // Act
            network.Edges.Add(new TrafficEdge { From = "A", To = "B", Capacity = 10 });
            network.Edges.Add(new TrafficEdge { From = "B", To = "C", Capacity = 15 });

            // Assert
            Assert.Equal(2, network.Edges.Count);
        }

        [Fact]
        public void TrafficGameResult_Should_StoreAllProperties()
        {
            // Arrange & Act
            var result = new TrafficGameResult
            {
                PlayerAnswer = 10,
                CorrectAnswer = 10,
                Status = "Correct",
                EdmondsKarpTime = 5,
                DinicTime = 3,
                TimeTaken = 5
            };

            // Assert
            Assert.Equal(10, result.PlayerAnswer);
            Assert.Equal(10, result.CorrectAnswer);
            Assert.Equal("Correct", result.Status);
            Assert.Equal(5, result.EdmondsKarpTime);
            Assert.Equal(3, result.DinicTime);
            Assert.Equal(5, result.TimeTaken);
        }

        [Theory]
        [InlineData(10, 10, "Correct")]
        [InlineData(5, 10, "Wrong")]
        [InlineData(15, 10, "Wrong")]
        [InlineData(0, 0, "Correct")]
        [InlineData(100, 100, "Correct")]
        public void CalculateMaxFlow_Should_HandleVariousAnswerScenarios(
            int playerAnswer, int expectedCorrectAnswer, string expectedStatus)
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "T", Capacity = expectedCorrectAnswer }
                }
            };
            var service = new TrafficGameService();

            // Act
            var result = service.CalculateMaxFlow(network, playerAnswer);

            // Assert
            Assert.Equal(expectedCorrectAnswer, result.CorrectAnswer);
            Assert.Equal(playerAnswer, result.PlayerAnswer);
            Assert.Equal(expectedStatus, result.Status);
        }

        [Fact]
        public void CalculateMaxFlow_Should_ProduceConsistentResultsOnMultipleCalls()
        {
            // Arrange
            var network = new TrafficNetwork
            {
                Edges = new List<TrafficEdge>
                {
                    new() { From = "A", To = "B", Capacity = 10 },
                    new() { From = "B", To = "C", Capacity = 5 },
                    new() { From = "C", To = "T", Capacity = 8 }
                }
            };
            var service = new TrafficGameService();

            // Act
            var result1 = service.CalculateMaxFlow(network, 5);
            var result2 = service.CalculateMaxFlow(network, 5);
            var result3 = service.CalculateMaxFlow(network, 5);

            // Assert - All results should be identical
            Assert.Equal(result1.CorrectAnswer, result2.CorrectAnswer);
            Assert.Equal(result2.CorrectAnswer, result3.CorrectAnswer);
            Assert.Equal("Correct", result1.Status);
            Assert.Equal("Correct", result2.Status);
            Assert.Equal("Correct", result3.Status);
        }
    }
}
