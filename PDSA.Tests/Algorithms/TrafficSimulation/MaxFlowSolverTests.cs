using PDSA.Core.Algorithms.TrafficSimulation;

namespace PDSA.Tests.Algorithms.TrafficSimulation
{
    public class MaxFlowSolverTests
    {
        [Fact]
        public void AddEdge_Should_AddEdgeWithCapacity()
        {
            // Arrange
            var solver = new MaxFlowSolver();

            // Act
            solver.AddEdge("A", "B", 10);

            // Assert - Edge should be added (verified by being able to compute flow)
            var flow = solver.EdmondsKarp("A", "B");
            Assert.Equal(10, flow);
        }

        [Fact]
        public void EdmondsKarp_Should_ReturnZeroForNoPath()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 10);
            solver.AddEdge("C", "D", 10);

            // Act - No path from A to D
            var flow = solver.EdmondsKarp("A", "D");

            // Assert
            Assert.Equal(0, flow);
        }

        [Fact]
        public void EdmondsKarp_Should_ComputeMaxFlowForSimplePath()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 10);
            solver.AddEdge("B", "C", 5);

            // Act - Bottleneck is 5
            var flow = solver.EdmondsKarp("A", "C");

            // Assert
            Assert.Equal(5, flow);
        }

        [Fact]
        public void EdmondsKarp_Should_HandleMultiplePaths()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            // Path 1: A -> B -> D (capacity 10)
            solver.AddEdge("A", "B", 10);
            solver.AddEdge("B", "D", 10);
            // Path 2: A -> C -> D (capacity 5)
            solver.AddEdge("A", "C", 5);
            solver.AddEdge("C", "D", 5);

            // Act - Total flow should be 15 (10 + 5)
            var flow = solver.EdmondsKarp("A", "D");

            // Assert
            Assert.Equal(15, flow);
        }

        [Fact]
        public void EdmondsKarp_Should_HandleBottleneck()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 100);
            solver.AddEdge("B", "C", 1); // Bottleneck
            solver.AddEdge("C", "D", 100);

            // Act
            var flow = solver.EdmondsKarp("A", "D");

            // Assert - Limited by bottleneck of 1
            Assert.Equal(1, flow);
        }

        [Fact]
        public void EdmondsKarp_Should_HandleComplexNetwork()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            // Classic max flow example
            solver.AddEdge("S", "A", 10);
            solver.AddEdge("S", "B", 5);
            solver.AddEdge("A", "B", 15);
            solver.AddEdge("A", "T", 10);
            solver.AddEdge("B", "T", 10);

            // Act
            var flow = solver.EdmondsKarp("S", "T");

            // Assert
            Assert.Equal(15, flow); // Max flow is 15
        }

        [Fact]
        public void Dinic_Should_ReturnZeroForNoPath()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 10);
            solver.AddEdge("C", "D", 10);

            // Act - No path from A to D
            var flow = solver.Dinic("A", "D");

            // Assert
            Assert.Equal(0, flow);
        }

        [Fact]
        public void Dinic_Should_ComputeMaxFlowForSimplePath()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 10);
            solver.AddEdge("B", "C", 5);

            // Act
            var flow = solver.Dinic("A", "C");

            // Assert
            Assert.Equal(5, flow);
        }

        [Fact]
        public void Dinic_Should_HandleMultiplePaths()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 10);
            solver.AddEdge("B", "D", 10);
            solver.AddEdge("A", "C", 5);
            solver.AddEdge("C", "D", 5);

            // Act
            var flow = solver.Dinic("A", "D");

            // Assert
            Assert.Equal(15, flow);
        }

        [Fact]
        public void Dinic_Should_HandleBottleneck()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 100);
            solver.AddEdge("B", "C", 1); // Bottleneck
            solver.AddEdge("C", "D", 100);

            // Act
            var flow = solver.Dinic("A", "D");

            // Assert
            Assert.Equal(1, flow);
        }

        [Fact]
        public void Dinic_Should_HandleComplexNetwork()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("S", "A", 10);
            solver.AddEdge("S", "B", 5);
            solver.AddEdge("A", "B", 15);
            solver.AddEdge("A", "T", 10);
            solver.AddEdge("B", "T", 10);

            // Act
            var flow = solver.Dinic("S", "T");

            // Assert
            Assert.Equal(15, flow);
        }

        [Fact]
        public void EdmondsKarpAndDinic_Should_ReturnSameMaxFlow()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 10);
            solver.AddEdge("A", "C", 10);
            solver.AddEdge("B", "D", 4);
            solver.AddEdge("B", "C", 2);
            solver.AddEdge("C", "D", 9);

            // Act
            var edmondKarpFlow = solver.EdmondsKarp("A", "D");
            var dinicFlow = solver.Dinic("A", "D");

            // Assert - Both algorithms should find the same max flow
            Assert.Equal(edmondKarpFlow, dinicFlow);
        }

        [Fact]
        public void ComputeMaxFlow_Should_UseEdmondsKarp()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 10);
            solver.AddEdge("B", "C", 5);

            // Act
            var legacyFlow = solver.ComputeMaxFlow("A", "C");
            var edmondsFlow = solver.EdmondsKarp("A", "C");

            // Assert - Legacy method should match Edmonds-Karp
            Assert.Equal(edmondsFlow, legacyFlow);
        }

        [Fact]
        public void EdmondsKarp_Should_HandleSingleEdge()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 25);

            // Act
            var flow = solver.EdmondsKarp("A", "B");

            // Assert
            Assert.Equal(25, flow);
        }

        [Fact]
        public void Dinic_Should_HandleSingleEdge()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 25);

            // Act
            var flow = solver.Dinic("A", "B");

            // Assert
            Assert.Equal(25, flow);
        }

        [Fact]
        public void EdmondsKarp_Should_HandleZeroCapacity()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 0);

            // Act
            var flow = solver.EdmondsKarp("A", "B");

            // Assert
            Assert.Equal(0, flow);
        }

        [Fact]
        public void Dinic_Should_HandleZeroCapacity()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 0);

            // Act
            var flow = solver.Dinic("A", "B");

            // Assert
            Assert.Equal(0, flow);
        }

        [Fact]
        public void EdmondsKarp_Should_HandleSelfLoop()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "A", 10);
            solver.AddEdge("A", "B", 5);

            // Act
            var flow = solver.EdmondsKarp("A", "B");

            // Assert - Self loop shouldn't affect flow to B
            Assert.Equal(5, flow);
        }

        [Fact]
        public void EdmondsKarp_Should_HandleCycles()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 10);
            solver.AddEdge("B", "C", 10);
            solver.AddEdge("C", "A", 10); // Cycle back
            solver.AddEdge("C", "D", 5);

            // Act
            var flow = solver.EdmondsKarp("A", "D");

            // Assert
            Assert.Equal(5, flow);
        }

        [Fact]
        public void Dinic_Should_HandleCycles()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 10);
            solver.AddEdge("B", "C", 10);
            solver.AddEdge("C", "A", 10); // Cycle
            solver.AddEdge("C", "D", 5);

            // Act
            var flow = solver.Dinic("A", "D");

            // Assert
            Assert.Equal(5, flow);
        }

        [Fact]
        public void AddEdge_Should_AllowMultipleEdgesFromSameNode()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 10);
            solver.AddEdge("A", "C", 15);
            solver.AddEdge("A", "D", 5);
            solver.AddEdge("B", "T", 10);
            solver.AddEdge("C", "T", 15);
            solver.AddEdge("D", "T", 5);

            // Act
            var flow = solver.EdmondsKarp("A", "T");

            // Assert - Should be sum of all paths (30)
            Assert.Equal(30, flow);
        }

        [Fact]
        public void EdmondsKarp_Should_HandleLargeCapacities()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 1000000);
            solver.AddEdge("B", "C", 999999);

            // Act
            var flow = solver.EdmondsKarp("A", "C");

            // Assert
            Assert.Equal(999999, flow);
        }

        [Fact]
        public void Dinic_Should_HandleLargeCapacities()
        {
            // Arrange
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 1000000);
            solver.AddEdge("B", "C", 999999);

            // Act
            var flow = solver.Dinic("A", "C");

            // Assert
            Assert.Equal(999999, flow);
        }

        [Fact]
        public void BothAlgorithms_Should_HandleTrafficNetworkExample()
        {
            // Arrange - Typical traffic network with intersections
            var solver = new MaxFlowSolver();
            solver.AddEdge("A", "B", 10); // A is source
            solver.AddEdge("A", "C", 10);
            solver.AddEdge("B", "D", 5);
            solver.AddEdge("C", "D", 10);
            solver.AddEdge("D", "T", 15); // T is sink

            // Act
            var edmondsKarpFlow = solver.EdmondsKarp("A", "T");
            var dinicFlow = solver.Dinic("A", "T");

            // Assert
            Assert.Equal(15, edmondsKarpFlow);
            Assert.Equal(15, dinicFlow);
            Assert.Equal(edmondsKarpFlow, dinicFlow);
        }
    }
}
