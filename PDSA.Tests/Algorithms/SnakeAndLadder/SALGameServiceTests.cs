using PDSA.Core.Algorithms.SnakeAndLadder;

namespace PDSA.Tests.Algorithms.SnakeAndLadder
{
    public class SALGameServiceTests
    {
        [Fact]
        public void SolveGame_Should_ReturnSolutionWithBothAlgorithms()
        {
            // Arrange
            var service = new SALGameService();
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
            var solution = service.SolveGame(request);

            // Assert
            Assert.NotNull(solution);
            Assert.NotEmpty(solution.GameId);
            Assert.Equal(2, solution.AlgorithmResults.Count);
        }

        [Fact]
        public void SolveGame_Should_ExecuteBFSAlgorithm()
        {
            // Arrange
            var service = new SALGameService();
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var solution = service.SolveGame(request);

            // Assert
            var bfsResult = solution.AlgorithmResults.FirstOrDefault(r => r.AlgorithmName.Contains("BFS"));
            Assert.NotNull(bfsResult);
            Assert.True(bfsResult.MinimumThrows > 0);
            Assert.True(bfsResult.ExecutionTimeMs >= 0);
        }

        [Fact]
        public void SolveGame_Should_ExecuteDPAlgorithm()
        {
            // Arrange
            var service = new SALGameService();
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var solution = service.SolveGame(request);

            // Assert
            var dpResult = solution.AlgorithmResults.FirstOrDefault(r => r.AlgorithmName.Contains("Dynamic Programming"));
            Assert.NotNull(dpResult);
            Assert.True(dpResult.MinimumThrows > 0);
            Assert.True(dpResult.ExecutionTimeMs >= 0);
        }

        [Fact]
        public void SolveGame_Should_HaveBothAlgorithmsAgree()
        {
            // Arrange
            var service = new SALGameService();
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>
                {
                    new Snake { Head = 70, Tail = 20 }
                },
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 10, Top = 50 }
                }
            };

            // Act
            var solution = service.SolveGame(request);

            // Assert
            var bfsResult = solution.AlgorithmResults[0];
            var dpResult = solution.AlgorithmResults[1];
            Assert.Equal(bfsResult.MinimumThrows, dpResult.MinimumThrows);
            Assert.Equal(bfsResult.MinimumThrows, solution.MinimumThrows);
        }

        [Fact]
        public void SolveGame_Should_ThrowForInvalidBoard()
        {
            // Arrange
            var service = new SALGameService();
            var request = new SALRequest
            {
                BoardSize = 1, // Invalid board size
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.SolveGame(request));
        }

        [Fact]
        public void SolveGame_Should_ThrowForInvalidSnake()
        {
            // Arrange
            var service = new SALGameService();
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>
                {
                    new Snake { Head = 10, Tail = 50 } // Invalid: head < tail
                },
                Ladders = new List<Ladder>()
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.SolveGame(request));
        }

        [Fact]
        public void SolveGame_Should_ReturnPathsForBothAlgorithms()
        {
            // Arrange
            var service = new SALGameService();
            var request = new SALRequest
            {
                BoardSize = 5,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var solution = service.SolveGame(request);

            // Assert
            foreach (var result in solution.AlgorithmResults)
            {
                Assert.NotNull(result.Path);
                Assert.NotEmpty(result.Path);
                Assert.Equal(1, result.Path[0]);
                Assert.Equal(25, result.Path[^1]); // 5x5 = 25
            }
        }

        [Fact]
        public void GenerateRandomBoard_Should_CreateValidBoard()
        {
            // Arrange
            var service = new SALGameService();

            // Act
            var request = service.GenerateRandomBoard(10, 5, 5);

            // Assert
            Assert.Equal(10, request.BoardSize);
            Assert.True(request.Snakes.Count <= 5);
            Assert.True(request.Ladders.Count <= 5);
            Assert.True(SALSolver.ValidateBoard(request));
        }

        [Fact]
        public void GenerateRandomBoard_Should_HaveNonOverlappingElements()
        {
            // Arrange
            var service = new SALGameService();

            // Act
            var request = service.GenerateRandomBoard(10, 3, 3);

            // Assert
            var positions = new HashSet<int>();
            
            foreach (var snake in request.Snakes)
            {
                Assert.DoesNotContain(snake.Head, positions);
                positions.Add(snake.Head);
            }
            
            foreach (var ladder in request.Ladders)
            {
                Assert.DoesNotContain(ladder.Bottom, positions);
                positions.Add(ladder.Bottom);
            }
        }

        [Fact]
        public void CheckUserAnswer_Should_ReturnTrueForCorrectAnswer()
        {
            // Arrange
            var service = new SALGameService();

            // Act
            var result = service.CheckUserAnswer(10, 10);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CheckUserAnswer_Should_ReturnFalseForWrongAnswer()
        {
            // Arrange
            var service = new SALGameService();

            // Act
            var result = service.CheckUserAnswer(10, 15);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetAlgorithmComparison_Should_ReturnComparisonString()
        {
            // Arrange
            var service = new SALGameService();
            var results = new List<SALAlgorithmResult>
            {
                new SALAlgorithmResult
                {
                    AlgorithmName = "BFS",
                    MinimumThrows = 10,
                    ExecutionTimeMs = 5.5
                },
                new SALAlgorithmResult
                {
                    AlgorithmName = "Dynamic Programming",
                    MinimumThrows = 10,
                    ExecutionTimeMs = 4.2
                }
            };

            // Act
            var comparison = service.GetAlgorithmComparison(results);

            // Assert
            Assert.NotEmpty(comparison);
            Assert.Contains("10 throws", comparison);
            Assert.Contains("ms", comparison);
        }

        [Fact]
        public void GetAlgorithmComparison_Should_HandleInsufficientResults()
        {
            // Arrange
            var service = new SALGameService();
            var results = new List<SALAlgorithmResult>
            {
                new SALAlgorithmResult { AlgorithmName = "BFS", MinimumThrows = 10 }
            };

            // Act
            var comparison = service.GetAlgorithmComparison(results);

            // Assert
            Assert.Contains("Insufficient", comparison);
        }

        [Fact]
        public void SolveGame_Should_RecordExecutionTimes()
        {
            // Arrange
            var service = new SALGameService();
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var solution = service.SolveGame(request);

            // Assert
            foreach (var result in solution.AlgorithmResults)
            {
                Assert.True(result.ExecutionTimeMs >= 0);
            }
        }

        [Fact]
        public void SolveGame_Should_AssignUniqueGameId()
        {
            // Arrange
            var service = new SALGameService();
            var request = new SALRequest
            {
                BoardSize = 5,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var solution1 = service.SolveGame(request);
            var solution2 = service.SolveGame(request);

            // Assert
            Assert.NotEqual(solution1.GameId, solution2.GameId);
        }

        [Fact]
        public void SolveGame_Should_HandleEmptySnakesAndLadders()
        {
            // Arrange
            var service = new SALGameService();
            var request = new SALRequest
            {
                BoardSize = 5,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Act
            var solution = service.SolveGame(request);

            // Assert
            Assert.NotNull(solution);
            Assert.True(solution.MinimumThrows > 0);
        }

        [Fact]
        public void GenerateRandomBoard_Should_CreateBoardWithRequestedSize()
        {
            // Arrange
            var service = new SALGameService();

            // Act
            var request = service.GenerateRandomBoard(8, 4, 4);

            // Assert
            Assert.Equal(8, request.BoardSize);
        }

        [Theory]
        [InlineData(5, 2, 2)]
        [InlineData(10, 5, 5)]
        [InlineData(15, 8, 8)]
        public void GenerateRandomBoard_Should_CreateValidBoardsForVariousSizes(int boardSize, int numSnakes, int numLadders)
        {
            // Arrange
            var service = new SALGameService();

            // Act
            var request = service.GenerateRandomBoard(boardSize, numSnakes, numLadders);

            // Assert
            Assert.Equal(boardSize, request.BoardSize);
            Assert.True(SALSolver.ValidateBoard(request));
        }

        [Fact]
        public void SolveGame_Should_HandleBoardWithOnlySnakes()
        {
            // Arrange
            var service = new SALGameService();
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>
                {
                    new Snake { Head = 50, Tail = 10 },
                    new Snake { Head = 70, Tail = 30 }
                },
                Ladders = new List<Ladder>()
            };

            // Act
            var solution = service.SolveGame(request);

            // Assert
            Assert.NotNull(solution);
            Assert.True(solution.MinimumThrows > 0);
        }

        [Fact]
        public void SolveGame_Should_HandleBoardWithOnlyLadders()
        {
            // Arrange
            var service = new SALGameService();
            var request = new SALRequest
            {
                BoardSize = 10,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>
                {
                    new Ladder { Bottom = 5, Top = 30 },
                    new Ladder { Bottom = 15, Top = 55 }
                }
            };

            // Act
            var solution = service.SolveGame(request);

            // Assert
            Assert.NotNull(solution);
            Assert.True(solution.MinimumThrows > 0);
        }

        [Fact]
        public void SALRequest_Should_InitializeWithEmptyLists()
        {
            // Act
            var request = new SALRequest();

            // Assert
            Assert.NotNull(request.Snakes);
            Assert.NotNull(request.Ladders);
            Assert.Empty(request.Snakes);
            Assert.Empty(request.Ladders);
        }

        [Fact]
        public void SALAlgorithmResult_Should_InitializeWithDefaults()
        {
            // Act
            var result = new SALAlgorithmResult();

            // Assert
            Assert.Equal(string.Empty, result.AlgorithmName);
            Assert.Equal(0, result.MinimumThrows);
            Assert.Equal(0, result.ExecutionTimeMs);
            Assert.NotNull(result.Path);
            Assert.Empty(result.Path);
        }

        [Fact]
        public void SALSolution_Should_InitializeWithDefaults()
        {
            // Act
            var solution = new SALSolution();

            // Assert
            Assert.Equal(string.Empty, solution.GameId);
            Assert.Equal(0, solution.MinimumThrows);
            Assert.NotNull(solution.AlgorithmResults);
            Assert.Empty(solution.AlgorithmResults);
        }

        [Fact]
        public void Snake_Should_StoreHeadAndTail()
        {
            // Arrange & Act
            var snake = new Snake { Head = 50, Tail = 10 };

            // Assert
            Assert.Equal(50, snake.Head);
            Assert.Equal(10, snake.Tail);
        }

        [Fact]
        public void Ladder_Should_StoreBottomAndTop()
        {
            // Arrange & Act
            var ladder = new Ladder { Bottom = 10, Top = 50 };

            // Assert
            Assert.Equal(10, ladder.Bottom);
            Assert.Equal(50, ladder.Top);
        }

        [Fact]
        public void GetAlgorithmComparison_Should_IdentifyFasterAlgorithm()
        {
            // Arrange
            var service = new SALGameService();
            var results = new List<SALAlgorithmResult>
            {
                new SALAlgorithmResult
                {
                    AlgorithmName = "BFS",
                    MinimumThrows = 10,
                    ExecutionTimeMs = 10.0
                },
                new SALAlgorithmResult
                {
                    AlgorithmName = "Dynamic Programming",
                    MinimumThrows = 10,
                    ExecutionTimeMs = 5.0
                }
            };

            // Act
            var comparison = service.GetAlgorithmComparison(results);

            // Assert
            Assert.Contains("Dynamic Programming", comparison);
            Assert.Contains("faster", comparison);
        }

        [Fact]
        public void SolveGame_Should_ProduceConsistentResultsForSameBoard()
        {
            // Arrange
            var service = new SALGameService();
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
            var solution1 = service.SolveGame(request);
            var solution2 = service.SolveGame(request);

            // Assert
            Assert.Equal(solution1.MinimumThrows, solution2.MinimumThrows);
        }

        [Fact]
        public void GenerateRandomBoard_Should_NotPlaceSnakeOrLadderOnCell1()
        {
            // Arrange
            var service = new SALGameService();

            // Act
            var request = service.GenerateRandomBoard(10, 5, 5);

            // Assert
            foreach (var snake in request.Snakes)
            {
                Assert.NotEqual(1, snake.Head);
                Assert.NotEqual(1, snake.Tail);
            }
            
            foreach (var ladder in request.Ladders)
            {
                Assert.NotEqual(1, ladder.Bottom);
            }
        }

        [Fact]
        public void GenerateRandomBoard_Should_NotPlaceSnakeOrLadderOnLastCell()
        {
            // Arrange
            var service = new SALGameService();
            int boardSize = 10;
            int lastCell = boardSize * boardSize;

            // Act
            var request = service.GenerateRandomBoard(boardSize, 5, 5);

            // Assert
            foreach (var snake in request.Snakes)
            {
                Assert.NotEqual(lastCell, snake.Head);
            }
            
            foreach (var ladder in request.Ladders)
            {
                Assert.NotEqual(lastCell, ladder.Bottom);
            }
        }
    }
}
