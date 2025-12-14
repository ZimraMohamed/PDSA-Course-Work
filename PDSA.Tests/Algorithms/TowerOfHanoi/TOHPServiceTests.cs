using PDSA.API.Models;
using PDSA.API.Services;
using PDSA.Core.Algorithms.TowerOfHanoi;

namespace PDSA.Tests.Algorithms.TowerOfHanoi
{
    public class TOHPServiceTests
    {
        [Fact]
        public void CheckUserMoves_Should_ReturnCorrectForOptimalThreePegSolution()
        {
            // Arrange
            var service = new TOHPService();
            var optimalMoves = TOHPSolver.SolveRecursive(3, 'A', 'C', 'B');
            var request = new TOHPRequest
            {
                NumPegs = 3,
                NumDisks = 3,
                UserMovesCount = optimalMoves.Count,
                UserSequence = string.Join(",", optimalMoves)
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.Equal(7, response.OptimalMoves); // 2^3 - 1 = 7
            Assert.True(response.CorrectMoves);
            Assert.True(response.CorrectSequence);
            Assert.Contains("correct", response.Message.ToLower());
        }

        [Fact]
        public void CheckUserMoves_Should_ReturnWrongForIncorrectMoveCount()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 3,
                NumDisks = 3,
                UserMovesCount = 10, // Wrong count (optimal is 7)
                UserSequence = "A → B,A → C,B → C,A → B,C → A,C → B,A → B"
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.Equal(7, response.OptimalMoves);
            Assert.False(response.CorrectMoves);
        }

        [Fact]
        public void CheckUserMoves_Should_ReturnWrongForInvalidSequence()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 3,
                NumDisks = 2,
                UserMovesCount = 3,
                UserSequence = "A → C,B → C,A → C" // Invalid: B has no disk initially
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.False(response.CorrectSequence);
        }

        [Fact]
        public void CheckUserMoves_Should_BenchmarkThreePegAlgorithms()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 3,
                NumDisks = 4,
                UserMovesCount = 15,
                UserSequence = string.Join(",", TOHPSolver.SolveRecursive(4, 'A', 'C', 'B'))
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.Contains("3-Peg Recursive", response.BenchmarkTimings.Keys);
            Assert.Contains("3-Peg Iterative", response.BenchmarkTimings.Keys);
            Assert.True(response.BenchmarkTimings["3-Peg Recursive"] >= 0);
            Assert.True(response.BenchmarkTimings["3-Peg Iterative"] >= 0);
        }

        [Fact]
        public void CheckUserMoves_Should_ReturnCorrectForOptimalFourPegSolution()
        {
            // Arrange
            var service = new TOHPService();
            var optimalMoves = TOHPSolver.Solve4Pegs_FrameStewart(4, 'A', 'D', 'B', 'C');
            var request = new TOHPRequest
            {
                NumPegs = 4,
                NumDisks = 4,
                UserMovesCount = optimalMoves.Count,
                UserSequence = string.Join(",", optimalMoves)
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.True(response.CorrectMoves);
            Assert.True(response.CorrectSequence);
            Assert.Contains("correct", response.Message.ToLower());
        }

        [Fact]
        public void CheckUserMoves_Should_BenchmarkFourPegAlgorithms()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 4,
                NumDisks = 5,
                UserMovesCount = 13,
                UserSequence = string.Join(",", TOHPSolver.Solve4Pegs_FrameStewart(5, 'A', 'D', 'B', 'C'))
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.Contains("4-Peg Frame-Stewart", response.BenchmarkTimings.Keys);
            Assert.Contains("4-Peg Balanced", response.BenchmarkTimings.Keys);
            Assert.True(response.BenchmarkTimings["4-Peg Frame-Stewart"] >= 0);
            Assert.True(response.BenchmarkTimings["4-Peg Balanced"] >= 0);
        }

        [Fact]
        public void CheckUserMoves_Should_ThrowForUnsupportedPegCount()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 5, // Unsupported
                NumDisks = 3,
                UserMovesCount = 7,
                UserSequence = "A → B"
            };

            // Act & Assert
            Assert.Throws<Exception>(() => service.CheckUserMoves(request));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 3)]
        [InlineData(3, 7)]
        [InlineData(4, 15)]
        [InlineData(5, 31)]
        public void CheckUserMoves_Should_CalculateCorrectOptimalMovesForThreePegs(int numDisks, int expectedMoves)
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 3,
                NumDisks = numDisks,
                UserMovesCount = expectedMoves,
                UserSequence = string.Join(",", TOHPSolver.SolveRecursive(numDisks, 'A', 'C', 'B'))
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.Equal(expectedMoves, response.OptimalMoves);
        }

        [Fact]
        public void CheckUserMoves_Should_ReturnOptimalSequenceList()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 3,
                NumDisks = 2,
                UserMovesCount = 3,
                UserSequence = "A → B,A → C,B → C"
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.NotNull(response.CorrectSequenceList);
            Assert.NotEmpty(response.CorrectSequenceList);
            Assert.Equal(3, response.CorrectSequenceList.Count);
        }

        [Fact]
        public void CheckUserMoves_Should_ReturnMessageForBothIncorrect()
        {
            // Arrange
            var service = new TOHPService();
            // Wrong move count and invalid sequence
            var request = new TOHPRequest
            {
                NumPegs = 3,
                NumDisks = 2,
                UserMovesCount = 10, // Wrong count
                UserSequence = "A → B,C → B,A → C" // Invalid sequence
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.Equal(3, response.OptimalMoves);
            Assert.False(response.CorrectMoves);
            Assert.False(response.CorrectSequence);
            Assert.Contains("incorrect", response.Message.ToLower());
        }

        [Fact]
        public void CheckUserMoves_Should_DetectCorrectMoveCountButInvalidSequence()
        {
            // Arrange
            var service = new TOHPService();
            // Right number of moves but invalid sequence
            var request = new TOHPRequest
            {
                NumPegs = 3,
                NumDisks = 2,
                UserMovesCount = 3,
                UserSequence = "A → B,C → B,A → C" // Invalid: C has no disk
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.True(response.CorrectMoves);
            Assert.False(response.CorrectSequence);
            Assert.Contains("invalid", response.Message.ToLower());
        }

        [Fact]
        public void CheckUserMoves_Should_HandleEmptyUserSequence()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 3,
                NumDisks = 2,
                UserMovesCount = 0,
                UserSequence = ""
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.False(response.CorrectSequence);
        }

        [Fact]
        public void CheckUserMoves_Should_SetAlgorithmNameForThreePegs()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 3,
                NumDisks = 2,
                UserMovesCount = 3,
                UserSequence = string.Join(",", TOHPSolver.SolveRecursive(2, 'A', 'C', 'B'))
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.NotEmpty(response.AlgorithmName);
            Assert.Contains("3-Peg", response.AlgorithmName);
        }

        [Fact]
        public void CheckUserMoves_Should_SetAlgorithmNameForFourPegs()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 4,
                NumDisks = 3,
                UserMovesCount = 5,
                UserSequence = string.Join(",", TOHPSolver.Solve4Pegs_FrameStewart(3, 'A', 'D', 'B', 'C'))
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.NotEmpty(response.AlgorithmName);
            Assert.Contains("4-Peg", response.AlgorithmName);
        }

        [Fact]
        public void CheckUserMoves_Should_RecordExecutionTime()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 3,
                NumDisks = 5,
                UserMovesCount = 31,
                UserSequence = string.Join(",", TOHPSolver.SolveRecursive(5, 'A', 'C', 'B'))
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.True(response.AlgorithmTimeMs >= 0);
        }

        [Fact]
        public void CheckUserMoves_Should_HandleCommaSpaceSeparatedSequence()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 3,
                NumDisks = 2,
                UserMovesCount = 3,
                UserSequence = "A → B, A → C, B → C" // With spaces after commas
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.True(response.CorrectSequence);
        }

        [Fact]
        public void TOHPRequest_Should_InitializeWithDefaults()
        {
            // Act
            var request = new TOHPRequest();

            // Assert
            Assert.Equal(0, request.NumPegs);
            Assert.Equal(0, request.NumDisks);
            Assert.Equal(0, request.UserMovesCount);
            Assert.Equal(string.Empty, request.UserSequence);
        }

        [Fact]
        public void TOHPResponse_Should_InitializeWithDefaults()
        {
            // Act
            var response = new TOHPResponse();

            // Assert
            Assert.Equal(0, response.OptimalMoves);
            Assert.Equal(0, response.UserMoves);
            Assert.False(response.CorrectMoves);
            Assert.False(response.CorrectSequence);
            Assert.Empty(response.CorrectSequenceList);
            Assert.Equal(string.Empty, response.Message);
            Assert.Equal(string.Empty, response.AlgorithmName);
            Assert.Equal(0, response.AlgorithmTimeMs);
            Assert.Empty(response.BenchmarkTimings);
        }

        [Fact]
        public void TOHPResponse_Should_StoreAllProperties()
        {
            // Arrange & Act
            var response = new TOHPResponse
            {
                OptimalMoves = 7,
                UserMoves = 7,
                CorrectMoves = true,
                CorrectSequence = true,
                CorrectSequenceList = new List<string> { "A → C" },
                Message = "Correct!",
                AlgorithmName = "3-Peg Recursive",
                AlgorithmTimeMs = 10,
                BenchmarkTimings = new Dictionary<string, long> { { "Test", 5 } }
            };

            // Assert
            Assert.Equal(7, response.OptimalMoves);
            Assert.Equal(7, response.UserMoves);
            Assert.True(response.CorrectMoves);
            Assert.True(response.CorrectSequence);
            Assert.Single(response.CorrectSequenceList);
            Assert.Equal("Correct!", response.Message);
            Assert.Equal("3-Peg Recursive", response.AlgorithmName);
            Assert.Equal(10, response.AlgorithmTimeMs);
            Assert.Single(response.BenchmarkTimings);
        }

        [Fact]
        public void CheckUserMoves_Should_HandleOneDiskPuzzle()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 3,
                NumDisks = 1,
                UserMovesCount = 1,
                UserSequence = "A → C"
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.Equal(1, response.OptimalMoves);
            Assert.True(response.CorrectMoves);
            Assert.True(response.CorrectSequence);
        }

        [Fact]
        public void CheckUserMoves_Should_Handle4PegsSingleDisk()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 4,
                NumDisks = 1,
                UserMovesCount = 1,
                UserSequence = "A → D"
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert
            Assert.Equal(1, response.OptimalMoves);
            Assert.True(response.CorrectMoves);
            Assert.True(response.CorrectSequence);
        }

        [Fact]
        public void CheckUserMoves_Should_CompareThreePegAlgorithmPerformance()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 3,
                NumDisks = 6,
                UserMovesCount = 63,
                UserSequence = string.Join(",", TOHPSolver.SolveRecursive(6, 'A', 'C', 'B'))
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert - Both algorithms should be benchmarked
            Assert.Equal(2, response.BenchmarkTimings.Count);
            Assert.True(response.BenchmarkTimings.ContainsKey("3-Peg Recursive"));
            Assert.True(response.BenchmarkTimings.ContainsKey("3-Peg Iterative"));
        }

        [Fact]
        public void CheckUserMoves_Should_CompareFourPegAlgorithmPerformance()
        {
            // Arrange
            var service = new TOHPService();
            var request = new TOHPRequest
            {
                NumPegs = 4,
                NumDisks = 6,
                UserMovesCount = 17,
                UserSequence = string.Join(",", TOHPSolver.Solve4Pegs_FrameStewart(6, 'A', 'D', 'B', 'C'))
            };

            // Act
            var response = service.CheckUserMoves(request);

            // Assert - Both 4-peg algorithms should be benchmarked
            Assert.Equal(2, response.BenchmarkTimings.Count);
            Assert.True(response.BenchmarkTimings.ContainsKey("4-Peg Frame-Stewart"));
            Assert.True(response.BenchmarkTimings.ContainsKey("4-Peg Balanced"));
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void CheckUserMoves_Should_ProduceConsistentResultsForSamePegCount(int pegCount)
        {
            // Arrange
            var service = new TOHPService();
            var numDisks = 3;
            var optimalSequence = pegCount == 3
                ? TOHPSolver.SolveRecursive(numDisks, 'A', 'C', 'B')
                : TOHPSolver.Solve4Pegs_FrameStewart(numDisks, 'A', 'D', 'B', 'C');

            var request = new TOHPRequest
            {
                NumPegs = pegCount == 3 ? 3 : 4,
                NumDisks = numDisks,
                UserMovesCount = optimalSequence.Count,
                UserSequence = string.Join(",", optimalSequence)
            };

            // Act
            var response1 = service.CheckUserMoves(request);
            var response2 = service.CheckUserMoves(request);

            // Assert - Results should be consistent
            Assert.Equal(response1.OptimalMoves, response2.OptimalMoves);
            Assert.Equal(response1.CorrectMoves, response2.CorrectMoves);
            Assert.Equal(response1.CorrectSequence, response2.CorrectSequence);
        }
    }
}
