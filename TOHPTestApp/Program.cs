using System;
using PDSA.Core.Algorithms.TowerOfHanoi;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Tower of Hanoi Test (Random Disk Count)");

        // Random disk count between 5 and 10
        Random rand = new Random();
        int numDisks = rand.Next(5, 11);
        Console.WriteLine($"Number of Disks: {numDisks}\n");

        // Recursive solut
        Console.WriteLine("Recursive Approach:");
        var recursiveMoves = TOHPSolver.SolveRecursive(numDisks, 'A', 'C', 'B');
        Console.WriteLine(string.Join(", ", recursiveMoves));
        Console.WriteLine($"Total Moves (Recursive): {recursiveMoves.Count}\n");

        // Iterative solution
        Console.WriteLine("Iterative Approach:");
        var iterativeMoves = TOHPSolver.SolveIterative(numDisks);
        Console.WriteLine(string.Join(", ", iterativeMoves));
        Console.WriteLine($"Total Moves (Iterative): {iterativeMoves.Count}");
    }
}
