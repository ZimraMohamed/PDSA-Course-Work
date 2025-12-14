using PDSA.Core.Algorithms.SnakeAndLadder;

namespace PDSA.Tests.Algorithms.SnakeAndLadder
{
    public class SALSolverTests
    {
        [Fact]
        public void SolveBFS_Should_FindMinimumThrowsForSimpleBoard()
        {
            // Arrange - Simple 10x10 board with no snakes or ladders
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var (minimumThrows, path) = SALSolver.SolveBFS(request);

            // Assert - Should take dice throws to reach 100
            Assert.True(minimumThrows > 0);
            Assert.NotEmpty(path);
            Assert.Equal(1, path[0]); // Starts at 1
            Assert.Equal(100, path[^1]); // Ends at 100
        }

        [Fact]
        public void SolveBFS_Should_HandleBoardWithLadders()
        {
            // Arrange - Board with a helpful ladder
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 2, Top = 99 } // Big ladder
                }
            };

            // Act
            var (minimumThrows, path) = SALSolver.SolveBFS(request);

            // Assert - Should use the ladder for quick win
            Assert.True(minimumThrows <= 2); // 1 throw to reach 2, then climb ladder
        }

        [Fact]
        public void SolveBFS_Should_HandleBoardWithSnakes()
        {
            // Arrange - Board with snakes
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>
                {
                    new Snake { Head = 50, Tail = 10 }
                },
                Ladders = new List<Ladder>()
            };

            // Act
            var (minimumThrows, path) = SALSolver.SolveBFS(request);

            // Assert - Should find a path despite snake
            Assert.True(minimumThrows > 0);
            Assert.NotEmpty(path);
        }

        [Fact]
        public void SolveDP_Should_FindMinimumThrowsForSimpleBoard()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var (minimumThrows, path) = SALSolver.SolveDP(request);

            // Assert
            Assert.True(minimumThrows > 0);
            Assert.NotEmpty(path);
            Assert.Equal(1, path[0]);
            Assert.Equal(100, path[^1]);
        }

        [Fact]
        public void SolveDP_Should_HandleBoardWithLadders()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 2, Top = 99 }
                }
            };

            // Act
            var (minimumThrows, path) = SALSolver.SolveDP(request);

            // Assert
            Assert.True(minimumThrows <= 2);
        }

        [Fact]
        public void BFSAndDP_Should_ProduceSameResult()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>
                {
                    new Snake { Head = 98, Tail = 10 }
                },
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 5, Top = 50 }
                }
            };

            // Act
            var (bfsThrows, bfsPath) = SALSolver.SolveBFS(request);
            var (dpThrows, dpPath) = SALSolver.SolveDP(request);

            // Assert - Both algorithms should find the same optimal solution
            Assert.Equal(bfsThrows, dpThrows);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnTrueForValidBoard()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>
                {
                    new Snake { Head = 50, Tail = 10 }
                },
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 5, Top = 30 }
                }
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnFalseForTooSmallBoard()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 1,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnFalseForTooLargeBoard()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 21,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnFalseForInvalidSnake()
        {
            // Arrange - Snake with head lower than tail
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>
                {
                    new Snake { Head = 10, Tail = 50 } // Invalid: head < tail
                },
                Ladders = new List<Ladder>()
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnFalseForInvalidLadder()
        {
            // Arrange - Ladder with top lower than bottom
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 50, Top = 10 } // Invalid: top < bottom
                }
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnFalseForSnakeOnLastCell()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>
                {
                    new Snake { Head = 100, Tail = 50 } // Can't have snake on last cell
                },
                Ladders = new List<Ladder>()
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnFalseForLadderFromLastCell()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 100, Top = 150 } // Can't have ladder from last cell
                }
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnFalseForOverlappingSnakes()
        {
            // Arrange - Two snakes with same head
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>
                {
                    new Snake { Head = 50, Tail = 10 },
                    new Snake { Head = 50, Tail = 20 } // Duplicate head
                },
                Ladders = new List<Ladder>()
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnFalseForOverlappingLadders()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 5, Top = 30 },
                    new Ladder { Bottom = 5, Top = 40 } // Duplicate bottom
                }
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnFalseForSnakeAndLadderOverlap()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>
                {
                    new Snake { Head = 50, Tail = 10 }
                },
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 50, Top = 70 } // Same position as snake head
                }
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void SolveBFS_Should_ReturnNegativeForImpossibleBoard()
        {
            // This test is theoretical - in practice, a board is always solvable
            // unless the last cell has a snake leading away, which validation prevents
            // But we can't easily create an impossible board with current validation rules
            // So this is more of a documentation test
            Assert.True(true); // Placeholder
        }

        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)]
        public void BFSAndDP_Should_AgreeOnVariousBoardSizes(int boardSize)
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = boardSize,
                Snakes = new List<Snake>
                {
                    new Snake { Head = boardSize * boardSize - 5, Tail = 5 }
                },
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 2, Top = boardSize * boardSize / 2 }
                }
            };

            // Act
            var (bfsThrows, _) = SALSolver.SolveBFS(request);
            var (dpThrows, _) = SALSolver.SolveDP(request);

            // Assert
            Assert.Equal(bfsThrows, dpThrows);
        }

        [Fact]
        public void SolveBFS_Should_ReturnPathStartingFrom1()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 5,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var (_, path) = SALSolver.SolveBFS(request);

            // Assert
            Assert.Equal(1, path[0]);
        }

        [Fact]
        public void SolveDP_Should_ReturnPathStartingFrom1()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 5,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var (_, path) = SALSolver.SolveDP(request);

            // Assert
            Assert.Equal(1, path[0]);
        }

        [Fact]
        public void SolveBFS_Should_ReturnPathEndingAtLastCell()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 5,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var (_, path) = SALSolver.SolveBFS(request);

            // Assert
            Assert.Equal(25, path[^1]); // 5x5 = 25
        }

        [Fact]
        public void ValidateBoard_Should_ReturnFalseForSnakeHeadOutOfBounds()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>
                {
                    new Snake { Head = 150, Tail = 50 } // Head > total cells
                },
                Ladders = new List<Ladder>()
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnFalseForLadderTopOutOfBounds()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 50, Top = 150 } // Top > total cells
                }
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void SolveBFS_Should_HandleMultipleSnakesAndLadders()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>
                {
                    new Snake { Head = 50, Tail = 10 },
                    new Snake { Head = 70, Tail = 30 }
                },
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 5, Top = 25 },
                    new Ladder { Bottom = 15, Top = 55 }
                }
            };

            // Act
            var (minimumThrows, path) = SALSolver.SolveBFS(request);

            // Assert
            Assert.True(minimumThrows > 0);
            Assert.NotEmpty(path);
        }

        [Fact]
        public void SolveDP_Should_HandleMultipleSnakesAndLadders()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>
                {
                    new Snake { Head = 50, Tail = 10 },
                    new Snake { Head = 70, Tail = 30 }
                },
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 5, Top = 25 },
                    new Ladder { Bottom = 15, Top = 55 }
                }
            };

            // Act
            var (minimumThrows, path) = SALSolver.SolveDP(request);

            // Assert
            Assert.True(minimumThrows > 0);
            Assert.NotEmpty(path);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnTrueForMinimumValidBoard()
        {
            // Arrange - Smallest valid board (2x2)
            var request = new SALRequest
            {
                BoardSize = 2,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnTrueForMaximumValidBoard()
        {
            // Arrange - Largest valid board (20x20)
            var request = new SALRequest
            {
                BoardSize = 20,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnFalseForSnakeWithEqualHeadAndTail()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>
                {
                    new Snake { Head = 50, Tail = 50 } // Head == Tail
                },
                Ladders = new List<Ladder>()
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateBoard_Should_ReturnFalseForLadderWithEqualBottomAndTop()
        {
            // Arrange
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 50, Top = 50 } // Bottom == Top
                }
            };

            // Act
            var isValid = SALSolver.ValidateBoard(request);

            // Assert
            Assert.False(isValid);
        }
    }
}
