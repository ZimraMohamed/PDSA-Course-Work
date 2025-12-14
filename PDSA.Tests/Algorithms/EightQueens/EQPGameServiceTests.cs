using PDSA.Core.Algorithms.EightQueens;

namespace PDSA.Tests.Algorithms.EightQueens
{
    public class EQPGameServiceTests
    {
        [Fact]
        public void CreateNewGameRound_Should_CreateRoundWithDefaultBoardSize()
        {
            // Act
            var round = EQPGameService.CreateNewGameRound();

            // Assert
            Assert.NotEqual(Guid.Empty, round.GameId);
            Assert.Equal(8, round.BoardSize);
        }

        [Fact]
        public void CreateNewGameRound_Should_CreateRoundWithCustomBoardSize()
        {
            // Act
            var round = EQPGameService.CreateNewGameRound(10);

            // Assert
            Assert.NotEqual(Guid.Empty, round.GameId);
            Assert.Equal(10, round.BoardSize);
        }

        [Fact]
        public void CreateNewGameRound_Should_GenerateUniqueGameIds()
        {
            // Act
            var round1 = EQPGameService.CreateNewGameRound();
            var round2 = EQPGameService.CreateNewGameRound();

            // Assert
            Assert.NotEqual(round1.GameId, round2.GameId);
        }

        [Fact]
        public void SolveAll_Should_ReturnSuccessForValidGame()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);

            // Act
            var result = EQPGameService.SolveAll(round.GameId);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public void SolveAll_Should_FindCorrectNumberOfSolutions()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);

            // Act
            var result = EQPGameService.SolveAll(round.GameId);

            // Assert
            Assert.Equal(92, result.SequentialSolutionsCount);
            Assert.Equal(92, result.ThreadedSolutionsCount);
        }

        [Fact]
        public void SolveAll_Should_ReturnBothAlgorithmResults()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);

            // Act
            var result = EQPGameService.SolveAll(round.GameId);

            // Assert
            Assert.Equal(2, result.AlgorithmResults.Count);
        }

        [Fact]
        public void SolveAll_Should_ContainSequentialAlgorithm()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);

            // Act
            var result = EQPGameService.SolveAll(round.GameId);

            // Assert
            var seqResult = result.AlgorithmResults.FirstOrDefault(r => r.AlgorithmName.Contains("Sequential"));
            Assert.NotNull(seqResult);
            Assert.Equal(92, seqResult.SolutionsFound);
            Assert.True(seqResult.ExecutionTimeMs >= 0);
        }

        [Fact]
        public void SolveAll_Should_ContainThreadedAlgorithm()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);

            // Act
            var result = EQPGameService.SolveAll(round.GameId);

            // Assert
            var thrResult = result.AlgorithmResults.FirstOrDefault(r => r.AlgorithmName.Contains("Threaded"));
            Assert.NotNull(thrResult);
            Assert.Equal(92, thrResult.SolutionsFound);
            Assert.True(thrResult.ExecutionTimeMs >= 0);
        }

        [Fact]
        public void SolveAll_Should_ReturnFailureForNonExistentGame()
        {
            // Arrange
            var nonExistentGameId = Guid.NewGuid();

            // Act
            var result = EQPGameService.SolveAll(nonExistentGameId);

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("not found", result.ErrorMessage);
        }

        [Fact]
        public void SolveAll_Should_IncludeRoundInformation()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);

            // Act
            var result = EQPGameService.SolveAll(round.GameId);

            // Assert
            Assert.NotNull(result.Round);
            Assert.Equal(round.GameId, result.Round.GameId);
            Assert.Equal(8, result.Round.BoardSize);
        }

        [Fact]
        public void ValidateSolution_Should_ReturnTrueForValidSolution()
        {
            // Arrange
            var positions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 1, Col = 4 },
                new QueenPosition { Row = 2, Col = 7 },
                new QueenPosition { Row = 3, Col = 5 },
                new QueenPosition { Row = 4, Col = 2 },
                new QueenPosition { Row = 5, Col = 6 },
                new QueenPosition { Row = 6, Col = 1 },
                new QueenPosition { Row = 7, Col = 3 }
            };

            // Act
            var isValid = EQPGameService.ValidateSolution(positions, 8);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateSolution_Should_ReturnFalseForInvalidSolution()
        {
            // Arrange - Two queens attacking each other
            var positions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 1, Col = 1 }, // Diagonal attack
                new QueenPosition { Row = 2, Col = 2 },
                new QueenPosition { Row = 3, Col = 3 },
                new QueenPosition { Row = 4, Col = 4 },
                new QueenPosition { Row = 5, Col = 5 },
                new QueenPosition { Row = 6, Col = 6 },
                new QueenPosition { Row = 7, Col = 7 }
            };

            // Act
            var isValid = EQPGameService.ValidateSolution(positions, 8);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateAndRecordPlayerSolution_Should_ReturnTrueForValidSolution()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);
            var positions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 1, Col = 4 },
                new QueenPosition { Row = 2, Col = 7 },
                new QueenPosition { Row = 3, Col = 5 },
                new QueenPosition { Row = 4, Col = 2 },
                new QueenPosition { Row = 5, Col = 6 },
                new QueenPosition { Row = 6, Col = 1 },
                new QueenPosition { Row = 7, Col = 3 }
            };

            // Act
            var isValid = EQPGameService.ValidateAndRecordPlayerSolution(round.GameId, "TestPlayer", positions);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateAndRecordPlayerSolution_Should_ReturnFalseForInvalidSolution()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);
            var positions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 1, Col = 0 }, // Same column
                new QueenPosition { Row = 2, Col = 2 },
                new QueenPosition { Row = 3, Col = 3 },
                new QueenPosition { Row = 4, Col = 4 },
                new QueenPosition { Row = 5, Col = 5 },
                new QueenPosition { Row = 6, Col = 6 },
                new QueenPosition { Row = 7, Col = 7 }
            };

            // Act
            var isValid = EQPGameService.ValidateAndRecordPlayerSolution(round.GameId, "TestPlayer", positions);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateAndRecordPlayerSolution_Should_ThrowForNonExistentGame()
        {
            // Arrange
            var nonExistentGameId = Guid.NewGuid();
            var positions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 }
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                EQPGameService.ValidateAndRecordPlayerSolution(nonExistentGameId, "TestPlayer", positions));
        }

        [Theory]
        [InlineData(4, 2)]
        [InlineData(5, 10)]
        [InlineData(6, 4)]
        [InlineData(8, 92)]
        public void SolveAll_Should_FindCorrectSolutionsForVariousBoardSizes(int boardSize, int expectedCount)
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(boardSize);

            // Act
            var result = EQPGameService.SolveAll(round.GameId);

            // Assert
            Assert.Equal(expectedCount, result.SequentialSolutionsCount);
            Assert.Equal(expectedCount, result.ThreadedSolutionsCount);
        }

        [Fact]
        public void SolveAll_Should_HaveConsistentAlgorithmResults()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);

            // Act
            var result = EQPGameService.SolveAll(round.GameId);

            // Assert
            Assert.Equal(result.SequentialSolutionsCount, result.AlgorithmResults[0].SolutionsFound);
            Assert.Equal(result.ThreadedSolutionsCount, result.AlgorithmResults[1].SolutionsFound);
        }

        [Fact]
        public void EQPGameRound_Should_InitializeWithDefaultValues()
        {
            // Act
            var round = new EQPGameRound();

            // Assert
            Assert.NotEqual(Guid.Empty, round.GameId);
            Assert.Equal(8, round.BoardSize);
        }

        [Fact]
        public void EQPAlgorithmResult_Should_InitializeWithDefaults()
        {
            // Act
            var result = new EQPAlgorithmResult();

            // Assert
            Assert.Equal(string.Empty, result.AlgorithmName);
            Assert.Equal(0, result.SolutionsFound);
            Assert.Equal(0, result.ExecutionTimeMs);
        }

        [Fact]
        public void EQPGameResult_Should_InitializeWithDefaults()
        {
            // Act
            var result = new EQPGameResult();

            // Assert
            Assert.NotNull(result.Round);
            Assert.Equal(0, result.SequentialSolutionsCount);
            Assert.Equal(0, result.ThreadedSolutionsCount);
            Assert.NotNull(result.AlgorithmResults);
            Assert.Empty(result.AlgorithmResults);
            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public void SolveAll_Should_RecordExecutionTimes()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);

            // Act
            var result = EQPGameService.SolveAll(round.GameId);

            // Assert
            Assert.All(result.AlgorithmResults, r => Assert.True(r.ExecutionTimeMs >= 0));
        }

        [Fact]
        public void SolveAll_Should_HandleSmallBoardSize()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(1);

            // Act
            var result = EQPGameService.SolveAll(round.GameId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(1, result.SequentialSolutionsCount);
            Assert.Equal(1, result.ThreadedSolutionsCount);
        }

        [Fact]
        public void SolveAll_Should_HandleBoardWithNoSolutions()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(2);

            // Act
            var result = EQPGameService.SolveAll(round.GameId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(0, result.SequentialSolutionsCount);
            Assert.Equal(0, result.ThreadedSolutionsCount);
        }

        [Fact]
        public void ValidateAndRecordPlayerSolution_Should_HandleNullPlayerName()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);
            var positions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 1, Col = 4 },
                new QueenPosition { Row = 2, Col = 7 },
                new QueenPosition { Row = 3, Col = 5 },
                new QueenPosition { Row = 4, Col = 2 },
                new QueenPosition { Row = 5, Col = 6 },
                new QueenPosition { Row = 6, Col = 1 },
                new QueenPosition { Row = 7, Col = 3 }
            };

            // Act
            var isValid = EQPGameService.ValidateAndRecordPlayerSolution(round.GameId, null, positions);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void CreateNewGameRound_Should_AcceptVariousBoardSizes()
        {
            // Act
            var round1 = EQPGameService.CreateNewGameRound(4);
            var round2 = EQPGameService.CreateNewGameRound(12);
            var round3 = EQPGameService.CreateNewGameRound(15);

            // Assert
            Assert.Equal(4, round1.BoardSize);
            Assert.Equal(12, round2.BoardSize);
            Assert.Equal(15, round3.BoardSize);
        }

        [Fact]
        public void SolveAll_Should_BothAlgorithmsAgree()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);

            // Act
            var result = EQPGameService.SolveAll(round.GameId);

            // Assert
            Assert.Equal(result.SequentialSolutionsCount, result.ThreadedSolutionsCount);
        }

        [Fact]
        public void ValidateSolution_Should_WorkForSmallBoardSize()
        {
            // Arrange - Valid 4 Queens solution
            var positions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 1 },
                new QueenPosition { Row = 1, Col = 3 },
                new QueenPosition { Row = 2, Col = 0 },
                new QueenPosition { Row = 3, Col = 2 }
            };

            // Act
            var isValid = EQPGameService.ValidateSolution(positions, 4);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void SolveAll_Should_ProduceConsistentResults()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);

            // Act
            var result1 = EQPGameService.SolveAll(round.GameId);
            var result2 = EQPGameService.SolveAll(round.GameId);

            // Assert
            Assert.Equal(result1.SequentialSolutionsCount, result2.SequentialSolutionsCount);
            Assert.Equal(result1.ThreadedSolutionsCount, result2.ThreadedSolutionsCount);
        }

        [Fact]
        public void ValidateSolution_Should_ReturnFalseForNull()
        {
            // Act
            var isValid = EQPGameService.ValidateSolution(null, 8);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void SolveAll_Should_IncludeAlgorithmNames()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);

            // Act
            var result = EQPGameService.SolveAll(round.GameId);

            // Assert
            Assert.Contains(result.AlgorithmResults, r => r.AlgorithmName.Contains("Sequential"));
            Assert.Contains(result.AlgorithmResults, r => r.AlgorithmName.Contains("Threaded"));
        }

        [Fact]
        public void ValidateAndRecordPlayerSolution_Should_AcceptValidMultipleSolutions()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);
            var positions1 = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 1, Col = 4 },
                new QueenPosition { Row = 2, Col = 7 },
                new QueenPosition { Row = 3, Col = 5 },
                new QueenPosition { Row = 4, Col = 2 },
                new QueenPosition { Row = 5, Col = 6 },
                new QueenPosition { Row = 6, Col = 1 },
                new QueenPosition { Row = 7, Col = 3 }
            };
            var positions2 = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 1 },
                new QueenPosition { Row = 1, Col = 3 },
                new QueenPosition { Row = 2, Col = 5 },
                new QueenPosition { Row = 3, Col = 7 },
                new QueenPosition { Row = 4, Col = 2 },
                new QueenPosition { Row = 5, Col = 0 },
                new QueenPosition { Row = 6, Col = 6 },
                new QueenPosition { Row = 7, Col = 4 }
            };

            // Act
            var isValid1 = EQPGameService.ValidateAndRecordPlayerSolution(round.GameId, "Player1", positions1);
            var isValid2 = EQPGameService.ValidateAndRecordPlayerSolution(round.GameId, "Player2", positions2);

            // Assert
            Assert.True(isValid1);
            Assert.True(isValid2);
        }

        [Fact]
        public void SolveAll_Should_CompleteWithinReasonableTime()
        {
            // Arrange
            var round = EQPGameService.CreateNewGameRound(8);

            // Act
            var result = EQPGameService.SolveAll(round.GameId);

            // Assert
            Assert.All(result.AlgorithmResults, r => Assert.True(r.ExecutionTimeMs < 10000)); // Under 10 seconds
        }
    }
}
