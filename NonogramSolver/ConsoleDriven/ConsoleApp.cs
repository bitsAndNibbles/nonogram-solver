using System;
using System.Threading;

namespace NonogramSolver.ConsoleDriven
{
   /// <summary>
   /// This main entry point will solve a nonogram puzzle and output
   /// the result to the console.
   /// </summary>
   class ConsoleApp
   {
      public static void Main(string[] args)
      {
         var solver = new Solver(PuzzleInput.Chicken);

         solver.Solve(CancellationToken.None);

         PrintSolution(solver.Solution);
      }

      private static void PrintSolution(CellStates[,] solution)
      {
         for (int row = 0; row < solution.GetLength(0); ++row)
         {
            for (int col = 0; col < solution.GetLength(1); ++col)
            {
               Console.Out.Write(((char)solution[row, col]));
            }
            Console.Out.WriteLine();
         }
      }

   }
}
