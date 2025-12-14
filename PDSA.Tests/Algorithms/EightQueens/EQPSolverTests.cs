using PDSA.Core.Algorithms.EightQueens;

namespace PDSA.Tests.Algorithms.EightQueens
{
    public class EQPSolverTests
    {
        [Fact]
        public void SolveSequential_Should_FindAllSolutionsFor8Queens()
        {
            // Act
            var (solutions, executionMs) = EQPSolver.SolveSequential(8);

            // Assert
            Assert.Equal(92, solutions.Count); // 8 Queens has 92 distinct solutions
            Assert.True(executionMs >= 0);
        }

        [Fact]
        public void SolveThreaded_Should_FindAllSolutionsFor8Queens()
        {
            // Act
            var (solutions, executionMs) = EQPSolver.SolveThreaded(8);

            // Assert
            Assert.Equal(92, solutions.Count);
            Assert.True(executionMs >= 0);
        }

        [Fact]
        public void SolveSequential_And_SolveThreaded_Should_FindSameNumberOfSolutions()
        {
            // Act
            var (seqSolutions, _) = EQPSolver.SolveSequential(8);
            var (thrSolutions, _) = EQPSolver.SolveThreaded(8);

            // Assert
            Assert.Equal(seqSolutions.Count, thrSolutions.Count);
        }

        [Theory]
        [InlineData(4, 2)]  // 4 Queens has 2 solutions
        [InlineData(5, 10)] // 5 Queens has 10 solutions
        [InlineData(6, 4)]  // 6 Queens has 4 solutions
        public void SolveSequential_Should_FindCorrectSolutionsForVariousBoardSizes(int n, int expectedCount)
        {
            // Act
            var (solutions, _) = EQPSolver.SolveSequential(n);

            // Assert
            Assert.Equal(expectedCount, solutions.Count);
        }

        [Theory]
        [InlineData(4, 2)]
        [InlineData(5, 10)]
        [InlineData(6, 4)]
        public void SolveThreaded_Should_FindCorrectSolutionsForVariousBoardSizes(int n, int expectedCount)
        {
            // Act
            var (solutions, _) = EQPSolver.SolveThreaded(n);

            // Assert
            Assert.Equal(expectedCount, solutions.Count);
        }

        [Fact]
        public void SolveSequential_Should_ReturnValidBoardRepresentation()
        {
            // Act
            var (solutions, _) = EQPSolver.SolveSequential(8);

            // Assert
            Assert.All(solutions, solution =>
            {
                Assert.Equal(8, solution.Length);
                Assert.All(solution, col => Assert.InRange(col, 0, 7));
            });
        }

        [Fact]
        public void SolveThreaded_Should_ReturnValidBoardRepresentation()
        {
            // Act
            var (solutions, _) = EQPSolver.SolveThreaded(8);

            // Assert
            Assert.All(solutions, solution =>
            {
                Assert.Equal(8, solution.Length);
                Assert.All(solution, col => Assert.InRange(col, 0, 7));
            });
        }

        [Fact]
        public void SolveSequential_Solutions_Should_BeValidPlacements()
        {
            // Act
            var (solutions, _) = EQPSolver.SolveSequential(8);

            // Assert
            foreach (var solution in solutions)
            {
                // Check no two queens attack each other
                for (int i = 0; i < 8; i++)
                {
                    for (int j = i + 1; j < 8; j++)
                    {
                        // Same column
                        Assert.NotEqual(solution[i], solution[j]);
                        // Same diagonal
                        Assert.NotEqual(Math.Abs(solution[i] - solution[j]), Math.Abs(i - j));
                    }
                }
            }
        }

        [Fact]
        public void SolveThreaded_Solutions_Should_BeValidPlacements()
        {
            // Act
            var (solutions, _) = EQPSolver.SolveThreaded(8);

            // Assert
            foreach (var solution in solutions)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = i + 1; j < 8; j++)
                    {
                        Assert.NotEqual(solution[i], solution[j]);
                        Assert.NotEqual(Math.Abs(solution[i] - solution[j]), Math.Abs(i - j));
                    }
                }
            }
        }

        [Fact]
        public void ValidateSolution_Should_ReturnTrueForValidSolution()
        {
            // Arrange - A known valid 8 Queens solution
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
            var isValid = EQPSolver.ValidateSolution(positions, 8);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateSolution_Should_ReturnFalseForInvalidSolution_SameColumn()
        {
            // Arrange - Two queens in same column
            var positions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 1, Col = 0 }, // Same column as row 0
                new QueenPosition { Row = 2, Col = 2 },
                new QueenPosition { Row = 3, Col = 3 },
                new QueenPosition { Row = 4, Col = 4 },
                new QueenPosition { Row = 5, Col = 5 },
                new QueenPosition { Row = 6, Col = 6 },
                new QueenPosition { Row = 7, Col = 7 }
            };

            // Act
            var isValid = EQPSolver.ValidateSolution(positions, 8);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateSolution_Should_ReturnFalseForInvalidSolution_SameDiagonal()
        {
            // Arrange - Two queens on same diagonal
            var positions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 1, Col = 1 }, // Diagonal attack with row 0
                new QueenPosition { Row = 2, Col = 2 },
                new QueenPosition { Row = 3, Col = 3 },
                new QueenPosition { Row = 4, Col = 4 },
                new QueenPosition { Row = 5, Col = 5 },
                new QueenPosition { Row = 6, Col = 6 },
                new QueenPosition { Row = 7, Col = 7 }
            };

            // Act
            var isValid = EQPSolver.ValidateSolution(positions, 8);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateSolution_Should_ReturnFalseForWrongNumberOfQueens()
        {
            // Arrange - Only 7 queens
            var positions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 1, Col = 2 },
                new QueenPosition { Row = 2, Col = 4 },
                new QueenPosition { Row = 3, Col = 6 },
                new QueenPosition { Row = 4, Col = 1 },
                new QueenPosition { Row = 5, Col = 3 },
                new QueenPosition { Row = 6, Col = 5 }
            };

            // Act
            var isValid = EQPSolver.ValidateSolution(positions, 8);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateSolution_Should_ReturnFalseForNullPositions()
        {
            // Act
            var isValid = EQPSolver.ValidateSolution(null, 8);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateSolution_Should_ReturnFalseForOutOfBoundsPosition()
        {
            // Arrange - Position out of board bounds
            var positions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 1, Col = 2 },
                new QueenPosition { Row = 2, Col = 4 },
                new QueenPosition { Row = 3, Col = 6 },
                new QueenPosition { Row = 4, Col = 1 },
                new QueenPosition { Row = 5, Col = 3 },
                new QueenPosition { Row = 6, Col = 5 },
                new QueenPosition { Row = 7, Col = 8 } // Out of bounds
            };

            // Act
            var isValid = EQPSolver.ValidateSolution(positions, 8);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateSolution_Should_ReturnFalseForDuplicateRows()
        {
            // Arrange - Two queens in same row
            var positions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 0, Col = 1 }, // Same row as first queen
                new QueenPosition { Row = 2, Col = 2 },
                new QueenPosition { Row = 3, Col = 3 },
                new QueenPosition { Row = 4, Col = 4 },
                new QueenPosition { Row = 5, Col = 5 },
                new QueenPosition { Row = 6, Col = 6 },
                new QueenPosition { Row = 7, Col = 7 }
            };

            // Act
            var isValid = EQPSolver.ValidateSolution(positions, 8);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void SolveSequential_Should_HaveUniqueColumns()
        {
            // Act
            var (solutions, _) = EQPSolver.SolveSequential(8);

            // Assert
            foreach (var solution in solutions)
            {
                var distinctColumns = solution.Distinct().Count();
                Assert.Equal(8, distinctColumns); // All columns should be unique
            }
        }

        [Fact]
        public void SolveThreaded_Should_HaveUniqueColumns()
        {
            // Act
            var (solutions, _) = EQPSolver.SolveThreaded(8);

            // Assert
            foreach (var solution in solutions)
            {
                var distinctColumns = solution.Distinct().Count();
                Assert.Equal(8, distinctColumns);
            }
        }

        [Fact]
        public void SolveSequential_Should_ReturnDistinctSolutions()
        {
            // Act
            var (solutions, _) = EQPSolver.SolveSequential(8);

            // Assert
            var signatures = solutions.Select(s => string.Join(",", s)).ToHashSet();
            Assert.Equal(solutions.Count, signatures.Count);
        }

        [Fact]
        public void SolveThreaded_Should_ReturnDistinctSolutions()
        {
            // Act
            var (solutions, _) = EQPSolver.SolveThreaded(8);

            // Assert
            var signatures = solutions.Select(s => string.Join(",", s)).ToHashSet();
            Assert.Equal(solutions.Count, signatures.Count);
        }

        [Fact]
        public void ValidateSolution_Should_ReturnTrueForAllSequentialSolutions()
        {
            // Arrange
            var (solutions, _) = EQPSolver.SolveSequential(8);

            // Act & Assert
            foreach (var solution in solutions)
            {
                var positions = solution.Select((col, row) => new QueenPosition { Row = row, Col = col }).ToList();
                Assert.True(EQPSolver.ValidateSolution(positions, 8));
            }
        }

        [Fact]
        public void ValidateSolution_Should_ReturnTrueForAllThreadedSolutions()
        {
            // Arrange
            var (solutions, _) = EQPSolver.SolveThreaded(8);

            // Act & Assert
            foreach (var solution in solutions)
            {
                var positions = solution.Select((col, row) => new QueenPosition { Row = row, Col = col }).ToList();
                Assert.True(EQPSolver.ValidateSolution(positions, 8));
            }
        }

        [Fact]
        public void SolveSequential_Should_Complete_For_SmallBoard()
        {
            // Act
            var (solutions, executionMs) = EQPSolver.SolveSequential(1);

            // Assert
            Assert.Single(solutions); // 1 Queen has exactly 1 solution
            Assert.True(executionMs >= 0);
        }

        [Fact]
        public void SolveSequential_Should_FindNoSolutions_For_2x2_Board()
        {
            // Act
            var (solutions, _) = EQPSolver.SolveSequential(2);

            // Assert
            Assert.Empty(solutions); // 2 Queens has no solution
        }

        [Fact]
        public void SolveSequential_Should_FindNoSolutions_For_3x3_Board()
        {
            // Act
            var (solutions, _) = EQPSolver.SolveSequential(3);

            // Assert
            Assert.Empty(solutions); // 3 Queens has no solution
        }

        [Fact]
        public void SolveThreaded_Should_FindNoSolutions_For_2x2_Board()
        {
            // Act
            var (solutions, _) = EQPSolver.SolveThreaded(2);

            // Assert
            Assert.Empty(solutions);
        }

        [Fact]
        public void SolveThreaded_Should_FindNoSolutions_For_3x3_Board()
        {
            // Act
            var (solutions, _) = EQPSolver.SolveThreaded(3);

            // Assert
            Assert.Empty(solutions);
        }

        [Fact]
        public void ValidateSolution_Should_HandleNegativePosition()
        {
            // Arrange
            var positions = new List<QueenPosition>
            {
                new QueenPosition { Row = -1, Col = 0 },
                new QueenPosition { Row = 1, Col = 2 },
                new QueenPosition { Row = 2, Col = 4 },
                new QueenPosition { Row = 3, Col = 6 },
                new QueenPosition { Row = 4, Col = 1 },
                new QueenPosition { Row = 5, Col = 3 },
                new QueenPosition { Row = 6, Col = 5 },
                new QueenPosition { Row = 7, Col = 7 }
            };

            // Act
            var isValid = EQPSolver.ValidateSolution(positions, 8);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void SolveSequential_ExecutionTime_Should_BeReasonable()
        {
            // Act
            var (_, executionMs) = EQPSolver.SolveSequential(8);

            // Assert
            Assert.True(executionMs < 5000); // Should complete in under 5 seconds
        }

        [Fact]
        public void SolveThreaded_ExecutionTime_Should_BeReasonable()
        {
            // Act
            var (_, executionMs) = EQPSolver.SolveThreaded(8);

            // Assert
            Assert.True(executionMs < 5000); // Should complete in under 5 seconds
        }

        [Fact]
        public void ValidateSolution_Should_WorkForSmallBoardSizes()
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
            var isValid = EQPSolver.ValidateSolution(positions, 4);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void SolveSequential_Should_ProduceConsistentResults()
        {
            // Act
            var (solutions1, _) = EQPSolver.SolveSequential(8);
            var (solutions2, _) = EQPSolver.SolveSequential(8);

            // Assert
            Assert.Equal(solutions1.Count, solutions2.Count);
        }

        [Fact]
        public void SolveThreaded_Should_ProduceConsistentResults()
        {
            // Act
            var (solutions1, _) = EQPSolver.SolveThreaded(8);
            var (solutions2, _) = EQPSolver.SolveThreaded(8);

            // Assert
            Assert.Equal(solutions1.Count, solutions2.Count);
        }

        [Fact]
        public void ValidateSolution_Should_ReturnFalseForEmptyList()
        {
            // Arrange
            var positions = new List<QueenPosition>();

            // Act
            var isValid = EQPSolver.ValidateSolution(positions, 8);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void QueenPosition_Should_StoreRowAndCol()
        {
            // Act
            var position = new QueenPosition { Row = 3, Col = 5 };

            // Assert
            Assert.Equal(3, position.Row);
            Assert.Equal(5, position.Col);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 2)]
        [InlineData(5, 10)]
        [InlineData(6, 4)]
        [InlineData(7, 40)]
        [InlineData(8, 92)]
        public void SolveSequential_Should_MatchKnownSolutionCounts(int n, int expectedCount)
        {
            // Act
            var (solutions, _) = EQPSolver.SolveSequential(n);

            // Assert
            Assert.Equal(expectedCount, solutions.Count);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 2)]
        [InlineData(5, 10)]
        [InlineData(6, 4)]
        [InlineData(7, 40)]
        [InlineData(8, 92)]
        public void SolveThreaded_Should_MatchKnownSolutionCounts(int n, int expectedCount)
        {
            // Act
            var (solutions, _) = EQPSolver.SolveThreaded(n);

            // Assert
            Assert.Equal(expectedCount, solutions.Count);
        }

        [Fact]
        public void ValidateSolution_Should_ReturnFalseForTooManyQueens()
        {
            // Arrange - 9 queens on 8x8 board
            var positions = new List<QueenPosition>
            {
                new QueenPosition { Row = 0, Col = 0 },
                new QueenPosition { Row = 1, Col = 2 },
                new QueenPosition { Row = 2, Col = 4 },
                new QueenPosition { Row = 3, Col = 6 },
                new QueenPosition { Row = 4, Col = 1 },
                new QueenPosition { Row = 5, Col = 3 },
                new QueenPosition { Row = 6, Col = 5 },
                new QueenPosition { Row = 7, Col = 7 },
                new QueenPosition { Row = 8, Col = 0 } // Extra queen
            };

            // Act
            var isValid = EQPSolver.ValidateSolution(positions, 8);

            // Assert
            Assert.False(isValid);
        }
    }
}
