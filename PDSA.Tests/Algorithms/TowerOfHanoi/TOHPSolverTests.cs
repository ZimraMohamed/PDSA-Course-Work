using PDSA.Core.Algorithms.TowerOfHanoi;

namespace PDSA.Tests.Algorithms.TowerOfHanoi
{
    public class TOHPSolverTests
    {
        [Theory]
        [InlineData(1, 1)]   // 2^1 - 1 = 1
        [InlineData(2, 3)]   // 2^2 - 1 = 3
        [InlineData(3, 7)]   // 2^3 - 1 = 7
        [InlineData(4, 15)]  // 2^4 - 1 = 15
        [InlineData(5, 31)]  // 2^5 - 1 = 31
        public void SolveRecursive_Should_ReturnCorrectNumberOfMoves(int numDisks, int expectedMoves)
        {
            // Act
            var moves = TOHPSolver.SolveRecursive(numDisks, 'A', 'C', 'B');

            // Assert
            Assert.Equal(expectedMoves, moves.Count);
        }

        [Fact]
        public void SolveRecursive_Should_ReturnEmptyListForZeroDisks()
        {
            // Act
            var moves = TOHPSolver.SolveRecursive(0, 'A', 'C', 'B');

            // Assert
            Assert.Empty(moves);
        }

        [Fact]
        public void SolveRecursive_Should_ReturnValidMovesForOneDisk()
        {
            // Act
            var moves = TOHPSolver.SolveRecursive(1, 'A', 'C', 'B');

            // Assert
            Assert.Single(moves);
            Assert.Contains("A", moves[0]);
            Assert.Contains("C", moves[0]);
        }

        [Fact]
        public void SolveRecursive_Should_ReturnValidSequenceForTwoDisks()
        {
            // Act
            var moves = TOHPSolver.SolveRecursive(2, 'A', 'C', 'B');

            // Assert
            Assert.Equal(3, moves.Count);
            // Expected sequence: A→B, A→C, B→C
            Assert.Contains("A", moves[0]);
            Assert.Contains("B", moves[0]);
        }

        [Fact]
        public void SolveRecursive_Should_HandleDifferentPegNames()
        {
            // Act
            var moves = TOHPSolver.SolveRecursive(2, 'X', 'Z', 'Y');

            // Assert
            Assert.Equal(3, moves.Count);
            Assert.All(moves, move => Assert.Matches(@"[XYZ]\s*→\s*[XYZ]", move));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 3)]
        [InlineData(3, 7)]
        [InlineData(4, 15)]
        [InlineData(5, 31)]
        public void SolveIterative_Should_ReturnCorrectNumberOfMoves(int numDisks, int expectedMoves)
        {
            // Act
            var moves = TOHPSolver.SolveIterative(numDisks);

            // Assert
            Assert.Equal(expectedMoves, moves.Count);
        }

        [Fact]
        public void SolveIterative_Should_ReturnEmptyListForZeroDisks()
        {
            // Act
            var moves = TOHPSolver.SolveIterative(0);

            // Assert
            Assert.Empty(moves);
        }

        [Fact]
        public void SolveIterative_Should_ProduceSameCountAsRecursive()
        {
            // Arrange
            for (int n = 1; n <= 6; n++)
            {
                // Act
                var recursiveMoves = TOHPSolver.SolveRecursive(n, 'A', 'C', 'B');
                var iterativeMoves = TOHPSolver.SolveIterative(n);

                // Assert
                Assert.Equal(recursiveMoves.Count, iterativeMoves.Count);
            }
        }

        [Fact]
        public void Solve4Pegs_FrameStewart_Should_ReturnFewerMovesThan3Pegs()
        {
            // Arrange
            int numDisks = 5;

            // Act
            var moves3Peg = TOHPSolver.SolveRecursive(numDisks, 'A', 'C', 'B');
            var moves4Peg = TOHPSolver.Solve4Pegs_FrameStewart(numDisks, 'A', 'D', 'B', 'C');

            // Assert - 4-peg should be more efficient
            Assert.True(moves4Peg.Count < moves3Peg.Count);
        }

        [Fact]
        public void Solve4Pegs_FrameStewart_Should_HandleZeroDisks()
        {
            // Act
            var moves = TOHPSolver.Solve4Pegs_FrameStewart(0, 'A', 'D', 'B', 'C');

            // Assert
            Assert.Empty(moves);
        }

        [Fact]
        public void Solve4Pegs_FrameStewart_Should_HandleOneDisk()
        {
            // Act
            var moves = TOHPSolver.Solve4Pegs_FrameStewart(1, 'A', 'D', 'B', 'C');

            // Assert
            Assert.Single(moves);
            Assert.Contains("A", moves[0]);
            Assert.Contains("D", moves[0]);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void Solve4Pegs_FrameStewart_Should_ReturnValidMovesForVariousDiskCounts(int numDisks)
        {
            // Act
            var moves = TOHPSolver.Solve4Pegs_FrameStewart(numDisks, 'A', 'D', 'B', 'C');

            // Assert
            Assert.NotEmpty(moves);
            Assert.All(moves, move => Assert.Matches(@"[ABCD]\s*→\s*[ABCD]", move));
        }

        [Fact]
        public void Solve4Pegs_Balanced_Should_ReturnFewerMovesThan3Pegs()
        {
            // Arrange
            int numDisks = 5;

            // Act
            var moves3Peg = TOHPSolver.SolveRecursive(numDisks, 'A', 'C', 'B');
            var moves4PegBalanced = TOHPSolver.Solve4Pegs_Balanced(numDisks, 'A', 'D', 'B', 'C');

            // Assert
            Assert.True(moves4PegBalanced.Count < moves3Peg.Count);
        }

        [Fact]
        public void Solve4Pegs_Balanced_Should_HandleZeroDisks()
        {
            // Act
            var moves = TOHPSolver.Solve4Pegs_Balanced(0, 'A', 'D', 'B', 'C');

            // Assert
            Assert.Empty(moves);
        }

        [Fact]
        public void Solve4Pegs_Balanced_Should_HandleOneDisk()
        {
            // Act
            var moves = TOHPSolver.Solve4Pegs_Balanced(1, 'A', 'D', 'B', 'C');

            // Assert
            Assert.Single(moves);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Solve4Pegs_Balanced_Should_ReturnValidMovesForVariousDiskCounts(int numDisks)
        {
            // Act
            var moves = TOHPSolver.Solve4Pegs_Balanced(numDisks, 'A', 'D', 'B', 'C');

            // Assert
            Assert.NotEmpty(moves);
            Assert.All(moves, move => Assert.Matches(@"[ABCD]\s*→\s*[ABCD]", move));
        }

        [Fact]
        public void ValidateSequence_Should_ReturnTrueForValidSequence()
        {
            // Arrange
            var validSequence = new List<string> { "A → C", "A → B", "C → B" };

            // Act
            var result = TOHPSolver.ValidateSequence(validSequence, 2, 3, 'A', 'B');

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateSequence_Should_ReturnFalseForNullSequence()
        {
            // Act
            var result = TOHPSolver.ValidateSequence(null, 3, 3, 'A', 'C');

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateSequence_Should_ReturnFalseForEmptySequence()
        {
            // Act
            var result = TOHPSolver.ValidateSequence(new List<string>(), 3, 3, 'A', 'C');

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateSequence_Should_ReturnFalseForInvalidMove()
        {
            // Arrange - Trying to move from empty peg
            var invalidSequence = new List<string> { "B → C" };

            // Act
            var result = TOHPSolver.ValidateSequence(invalidSequence, 2, 3, 'A', 'C');

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateSequence_Should_ReturnFalseForIllegalMove()
        {
            // Arrange - Trying to place larger disk on smaller disk
            var invalidSequence = new List<string> { "A → B", "A → B" }; // This will try to place disk 1 on disk 2

            // Act
            var result = TOHPSolver.ValidateSequence(invalidSequence, 2, 3, 'A', 'C');

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateSequence_Should_ReturnFalseForIncompleteSequence()
        {
            // Arrange - Only one move for 2 disks (needs 3)
            var incompleteSequence = new List<string> { "A → B" };

            // Act
            var result = TOHPSolver.ValidateSequence(incompleteSequence, 2, 3, 'A', 'C');

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateSequence_Should_ValidateRecursiveSolution()
        {
            // Arrange
            int numDisks = 3;
            var solution = TOHPSolver.SolveRecursive(numDisks, 'A', 'C', 'B');

            // Act
            var result = TOHPSolver.ValidateSequence(solution, numDisks, 3, 'A', 'C');

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateSequence_Should_ValidateIterativeSolution()
        {
            // Arrange
            int numDisks = 3;
            var solution = TOHPSolver.SolveIterative(numDisks);

            // Act
            var result = TOHPSolver.ValidateSequence(solution, numDisks, 3, 'A', 'C');

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateSequence_Should_ValidateFrameStewartSolution()
        {
            // Arrange
            int numDisks = 4;
            var solution = TOHPSolver.Solve4Pegs_FrameStewart(numDisks, 'A', 'D', 'B', 'C');

            // Act
            var result = TOHPSolver.ValidateSequence(solution, numDisks, 4, 'A', 'D');

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateSequence_Should_ValidateBalancedSolution()
        {
            // Arrange
            int numDisks = 4;
            var solution = TOHPSolver.Solve4Pegs_Balanced(numDisks, 'A', 'D', 'B', 'C');

            // Act
            var result = TOHPSolver.ValidateSequence(solution, numDisks, 4, 'A', 'D');

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateSequence_Should_HandleDifferentMoveFormats()
        {
            // Arrange - Various arrow formats
            var sequence1 = new List<string> { "A → C" };
            var sequence2 = new List<string> { "A->C" };
            var sequence3 = new List<string> { "A>C" };

            // Act & Assert
            Assert.True(TOHPSolver.ValidateSequence(sequence1, 1, 3, 'A', 'C'));
            Assert.True(TOHPSolver.ValidateSequence(sequence2, 1, 3, 'A', 'C'));
            Assert.True(TOHPSolver.ValidateSequence(sequence3, 1, 3, 'A', 'C'));
        }

        [Fact]
        public void ValidateSequence_Should_ReturnFalseForInvalidPegCount()
        {
            // Arrange
            var sequence = new List<string> { "A → B" };

            // Act
            var result = TOHPSolver.ValidateSequence(sequence, 1, 5, 'A', 'B');

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateSequence_Should_ReturnFalseForNonExistentPeg()
        {
            // Arrange
            var sequence = new List<string> { "A → Z" };

            // Act
            var result = TOHPSolver.ValidateSequence(sequence, 1, 3, 'A', 'C');

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateSequence_Should_ReturnFalseForMalformedMove()
        {
            // Arrange
            var sequence = new List<string> { "A B C" };

            // Act
            var result = TOHPSolver.ValidateSequence(sequence, 1, 3, 'A', 'C');

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void SolveRecursive_Should_ProduceValidSequenceForAllDiskCounts(int numDisks)
        {
            // Act
            var moves = TOHPSolver.SolveRecursive(numDisks, 'A', 'C', 'B');
            var isValid = TOHPSolver.ValidateSequence(moves, numDisks, 3, 'A', 'C');

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void SolveIterative_Should_ProduceValidSequenceForAllDiskCounts(int numDisks)
        {
            // Act
            var moves = TOHPSolver.SolveIterative(numDisks);
            var isValid = TOHPSolver.ValidateSequence(moves, numDisks, 3, 'A', 'C');

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Solve4Pegs_FrameStewart_Should_BeMoreEfficientThanBalanced_ForLargerProblems()
        {
            // Arrange
            int numDisks = 8;

            // Act
            var frameStewartMoves = TOHPSolver.Solve4Pegs_FrameStewart(numDisks, 'A', 'D', 'B', 'C');
            var balancedMoves = TOHPSolver.Solve4Pegs_Balanced(numDisks, 'A', 'D', 'B', 'C');

            // Assert - Frame-Stewart should be more efficient (or equal)
            Assert.True(frameStewartMoves.Count <= balancedMoves.Count);
        }

        [Fact]
        public void SolveRecursive_Should_UseAllThreePegs()
        {
            // Arrange
            int numDisks = 3;

            // Act
            var moves = TOHPSolver.SolveRecursive(numDisks, 'A', 'C', 'B');

            // Assert - Should use all three pegs
            var allMoves = string.Join(" ", moves);
            Assert.Contains("A", allMoves);
            Assert.Contains("B", allMoves);
            Assert.Contains("C", allMoves);
        }

        [Fact]
        public void Solve4Pegs_Should_UseAllFourPegs()
        {
            // Arrange
            int numDisks = 5;

            // Act
            var moves = TOHPSolver.Solve4Pegs_FrameStewart(numDisks, 'A', 'D', 'B', 'C');

            // Assert - Should potentially use all four pegs
            var allMoves = string.Join(" ", moves);
            Assert.Contains("A", allMoves);
            Assert.Contains("D", allMoves);
        }

        [Fact]
        public void ValidateSequence_Should_ReturnFalseForDisksOnWrongPeg()
        {
            // Arrange - Sequence that leaves disks on wrong peg
            var sequence = new List<string> { "A → B" };

            // Act - Expecting all disks on C, but they're on B
            var result = TOHPSolver.ValidateSequence(sequence, 1, 3, 'A', 'C');

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void SolveIterative_Should_HandleEvenNumberOfDisks()
        {
            // Act
            var moves = TOHPSolver.SolveIterative(4);
            var isValid = TOHPSolver.ValidateSequence(moves, 4, 3, 'A', 'C');

            // Assert
            Assert.Equal(15, moves.Count);
            Assert.True(isValid);
        }

        [Fact]
        public void SolveIterative_Should_HandleOddNumberOfDisks()
        {
            // Act
            var moves = TOHPSolver.SolveIterative(5);
            var isValid = TOHPSolver.ValidateSequence(moves, 5, 3, 'A', 'C');

            // Assert
            Assert.Equal(31, moves.Count);
            Assert.True(isValid);
        }
    }
}
