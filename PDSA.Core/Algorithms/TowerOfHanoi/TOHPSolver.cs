using System;
using System.Collections.Generic;

namespace PDSA.Core.Algorithms.TowerOfHanoi
{
    public class TOHPSolver
    {
        // Recursive solution
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

        // Iterative solution for 3 pegs
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
    }
}
