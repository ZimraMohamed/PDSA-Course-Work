using System;
using System.Collections.Generic;

namespace PDSA.Core.Algorithms.TowerOfHanoi
{
    public class TOHPSolver
    {
        /* ============================================================
           3 PEG ALGORITHMS
           ============================================================ */

        // ------------------------------------------------------------
        //  Recursive solution for classic 3-peg Tower of Hanoi
        // ------------------------------------------------------------
        public static List<string> SolveRecursive(int n, char source, char target, char auxiliary)
        {
            var moves = new List<string>();
            SolveRecursiveHelper(n, source, target, auxiliary, moves);
            return moves;
        }

        private static void SolveRecursiveHelper(int n, char source, char target, char auxiliary, List<string> moves)
        {
            if (n == 0) return;

            SolveRecursiveHelper(n - 1, source, auxiliary, target, moves);
            moves.Add($"{source} → {target}");
            SolveRecursiveHelper(n - 1, auxiliary, target, source, moves);
        }


        // ------------------------------------------------------------
        //  Iterative solution for classic 3-peg Tower of Hanoi
        // ------------------------------------------------------------
        public static List<string> SolveIterative(int n)
        {
            var moves = new List<string>();
            int totalMoves = (int)Math.Pow(2, n) - 1;

            var pegA = new Stack<int>();
            var pegB = new Stack<int>();
            var pegC = new Stack<int>();

            for (int i = n; i >= 1; i--) pegA.Push(i);

            Stack<int> src = pegA, aux = pegB, tgt = pegC;
            char s = 'A', a = 'B', t = 'C';

            if (n % 2 == 0)
            {
                var tempStack = tgt; tgt = aux; aux = tempStack;
                var tempChar = t; t = a; a = tempChar;
            }

            for (int i = 1; i <= totalMoves; i++)
            {
                if (i % 3 == 1) MakeLegalMove(src, tgt, s, t, moves);
                else if (i % 3 == 2) MakeLegalMove(src, aux, s, a, moves);
                else if (i % 3 == 0) MakeLegalMove(aux, tgt, a, t, moves);
            }

            return moves;
        }

        private static void MakeLegalMove(Stack<int> from, Stack<int> to, char fromName, char toName, List<string> moves)
        {
            int fromTop = from.Count > 0 ? from.Peek() : int.MaxValue;
            int toTop = to.Count > 0 ? to.Peek() : int.MaxValue;

            if (fromTop < toTop)
            {
                to.Push(from.Pop());
                moves.Add($"{fromName} → {toName}");
            }
            else
            {
                from.Push(to.Pop());
                moves.Add($"{toName} → {fromName}");
            }
        }

        /* ============================================================
           4 PEG ALGORITHMS
           ============================================================ */

        // ------------------------------------------------------------
        //  Algorithm 1: Frame–Stewart optimal recursive algorithm
        //  This is the mathematically optimal solution for 4 pegs.
        // ------------------------------------------------------------
        public static List<string> Solve4Pegs_FrameStewart(int n, char source, char target, char aux1, char aux2)
        {
            var moves = new List<string>();
            Solve4Pegs_FS_Helper(n, source, target, aux1, aux2, moves);
            return moves;
        }

        private static void Solve4Pegs_FS_Helper(int n, char s, char t, char a1, char a2, List<string> moves)
        {
            if (n == 0) return;

            // Optimal split approximation: n - round(sqrt(2n + 1)) + 1
            int k = n - (int)Math.Round(Math.Sqrt(2 * n + 1)) + 1;
            if (k < 1) k = n - 1;

            // Step 1: Move top k disks to auxiliary peg
            Solve4Pegs_FS_Helper(k, s, a1, a2, t, moves);

            // Step 2: Move remaining disks using 3-peg method
            SolveRecursiveHelper(n - k, s, t, a2, moves);

            // Step 3: Move k disks from auxiliary to target
            Solve4Pegs_FS_Helper(k, a1, t, s, a2, moves);
        }


        // ------------------------------------------------------------
        //  Algorithm 2: Balanced Split 4-peg algorithm
        //  Simpler than Frame–Stewart but still works consistently.
        // ------------------------------------------------------------
        public static List<string> Solve4Pegs_Balanced(int n, char source, char target, char aux1, char aux2)
        {
            var moves = new List<string>();
            Solve4Pegs_Balanced_Helper(n, source, target, aux1, aux2, moves);
            return moves;
        }

        private static void Solve4Pegs_Balanced_Helper(int n, char s, char t, char a1, char a2, List<string> moves)
        {
            if (n == 0) return;

            if (n == 1)
            {
                moves.Add($"{s} → {t}");
                return;
            }

            // Balanced split: move top n-2 to aux1, then 2 using 3-pegs, then rest.
            int k = n - 2;

            // Step 1: Move k disks to aux1 using 4 pegs
            Solve4Pegs_Balanced_Helper(k, s, a1, a2, t, moves);

            // Step 2: Move 2 disks using classic 3-peg method
            SolveRecursiveHelper(2, s, t, a2, moves);

            // Step 3: Move k disks from aux1 to target
            Solve4Pegs_Balanced_Helper(k, a1, t, s, a2, moves);
        }

        /* ============================================================
           SEQUENCE VALIDATOR
           Validates if a user's move sequence is actually correct
           (solves the puzzle legally) rather than just comparing strings
           ============================================================ */

        public static bool ValidateSequence(List<string> userMoves, int numDisks, int numPegs, char source, char target)
        {
            if (userMoves == null || userMoves.Count == 0)
                return false;

            // Initialize pegs with stacks
            var pegs = new Dictionary<char, Stack<int>>();
            
            // Create pegs based on numPegs (3 or 4)
            pegs[source] = new Stack<int>();
            pegs[target] = new Stack<int>();
            
            if (numPegs == 3)
            {
                // For 3 pegs: A, C, and one auxiliary (B)
                char aux = 'B';
                if (source == 'B' || target == 'B') aux = 'C';
                if (source == 'C' || target == 'C') aux = 'B';
                pegs[aux] = new Stack<int>();
            }
            else if (numPegs == 4)
            {
                // For 4 pegs: A, D, and two auxiliaries (B, C)
                pegs['B'] = new Stack<int>();
                pegs['C'] = new Stack<int>();
            }
            else
            {
                return false;
            }

            // Place all disks on source peg (largest at bottom)
            for (int i = numDisks; i >= 1; i--)
            {
                pegs[source].Push(i);
            }

            // Process each move
            foreach (var move in userMoves)
            {
                // Parse move (format: "A → B" or "A→B")
                var parts = move.Split(new[] { '→', '-', '>' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    return false;

                char from = parts[0].Trim()[0];
                char to = parts[1].Trim()[0];

                // Check if pegs exist
                if (!pegs.ContainsKey(from) || !pegs.ContainsKey(to))
                    return false;

                // Check if source peg has a disk
                if (pegs[from].Count == 0)
                    return false;

                // Get disk from source
                int disk = pegs[from].Peek();

                // Check if move is legal (can't place larger disk on smaller disk)
                if (pegs[to].Count > 0 && pegs[to].Peek() < disk)
                    return false;

                // Make the move
                pegs[from].Pop();
                pegs[to].Push(disk);
            }

            // Check if all disks are on the target peg
            if (pegs[target].Count != numDisks)
                return false;

            // Check if disks are in correct order (largest at bottom)
            var targetStack = pegs[target].ToArray();
            for (int i = 0; i < targetStack.Length - 1; i++)
            {
                if (targetStack[i] > targetStack[i + 1])
                    return false;
            }

            // All checks passed - sequence is valid!
            return true;
        }
    }
}