using System;
using System.Diagnostics;
using PDSA.Core.Algorithms.TowerOfHanoi;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Tower of Hanoi Test (Random Disk Count)");

        // Ask for number of pegs
        int numPegs;
        while (true)
        {
            Console.Write("Enter number of pegs (3 or 4): ");
            if (int.TryParse(Console.ReadLine(), out numPegs) && (numPegs == 3 || numPegs == 4))
                break;
            Console.WriteLine("Invalid input. Please enter 3 or 4.");
        }

        // Random disk count between 5 and 10
        Random rand = new Random();
        int numDisks = rand.Next(5, 11);
        Console.WriteLine($"\nNumber of Disks: {numDisks}\n");

        if (numPegs == 3)
        {
            // 3-Peg Recursive
            Console.WriteLine("3-Peg Recursive Approach:");
            Stopwatch sw = Stopwatch.StartNew();
            var recursiveMoves = TOHPSolver.SolveRecursive(numDisks, 'A', 'C', 'B');
            sw.Stop();
            Console.WriteLine(string.Join(", ", recursiveMoves));
            Console.WriteLine($"Total Moves (Recursive): {recursiveMoves.Count}");
            Console.WriteLine($"Time Taken: {sw.Elapsed.TotalMilliseconds} ms\n");

            // 3-Peg Iterative
            Console.WriteLine("3-Peg Iterative Approach:");
            sw.Restart();
            var iterativeMoves = TOHPSolver.SolveIterative(numDisks);
            sw.Stop();
            Console.WriteLine(string.Join(", ", iterativeMoves));
            Console.WriteLine($"Total Moves (Iterative): {iterativeMoves.Count}");
            Console.WriteLine($"Time Taken: {sw.Elapsed.TotalMilliseconds} ms");
        }
        else if (numPegs == 4)
        {
            // 4-Peg Frame–Stewart
            Console.WriteLine("4-Peg Frame–Stewart Recursive Approach:");
            Stopwatch sw = Stopwatch.StartNew();
            var fsMoves = TOHPSolver.Solve4Pegs_FrameStewart(numDisks, 'A', 'D', 'B', 'C');
            sw.Stop();
            Console.WriteLine(string.Join(", ", fsMoves));
            Console.WriteLine($"Total Moves (Frame–Stewart): {fsMoves.Count}");
            Console.WriteLine($"Time Taken: {sw.Elapsed.TotalMilliseconds} ms\n");

            // 4-Peg Balanced Split
            Console.WriteLine("4-Peg Balanced Split Approach:");
            sw.Restart();
            var balancedMoves = TOHPSolver.Solve4Pegs_Balanced(numDisks, 'A', 'D', 'B', 'C');
            sw.Stop();
            Console.WriteLine(string.Join(", ", balancedMoves));
            Console.WriteLine($"Total Moves (Balanced Split): {balancedMoves.Count}");
            Console.WriteLine($"Time Taken: {sw.Elapsed.TotalMilliseconds} ms");
        }
    }
}
