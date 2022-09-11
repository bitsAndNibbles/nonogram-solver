using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static NonogramSolver.CellStates;

namespace NonogramSolver
{
   internal class Solver
   {
      private readonly Constraint[] mRowConstraints;
      private readonly Constraint[] mColConstraints;

      internal CellStates[,] Solution { get; }

      internal int Width
      {
         get
         {
            return mColConstraints.Length;
         }
      }

      internal int Height
      {
         get
         {
            return mRowConstraints.Length;
         }
      }

      internal Solver(PuzzleInput input)
      {
         mRowConstraints = ConvertToConstraintObjs(input.RowConstraints, ConstraintTypes.Row);
         mColConstraints = ConvertToConstraintObjs(input.ColumnConstraints, ConstraintTypes.Column);

         Solution = new CellStates[Height, Width];
      }

      private static Constraint[] ConvertToConstraintObjs(int[][] rawRules, ConstraintTypes type)
      {
         Constraint[] constraints = new Constraint[rawRules.Length];

         for (int idx = 0; idx < rawRules.Length; idx++)
         {
            var constraint = new Constraint(
               rawRules[idx],
               type,
               idx);
            constraints[idx] = constraint;
         }

         return constraints;
      }

      internal async Task SolveAsync(CancellationToken cancelToken)
      {
         await Task.Run(() => Solve(cancelToken), cancelToken)
            .ConfigureAwait(continueOnCapturedContext: false);
      }

      internal void Solve(CancellationToken cancelToken)
      {
         // add all constraints to a queue, ordered by rules with largest number of occupied squares
         var constraints = new SortedSet<Constraint>(mRowConstraints.Concat(mColConstraints));

         // loop through queue until empty
         while (constraints.Any() && !cancelToken.IsCancellationRequested)
         {
            var constraint = constraints.First();
            constraints.Remove(constraint);

            if (constraint.Type == ConstraintTypes.Column)
            {
               int w_constraint_no = constraint.Position;
               var shadings = Shadings.GetAllShadings(
                  constraint.ConstraintNums,
                  Height,
                  GetSolutionCol(w_constraint_no),
                  cancelToken);

               cancelToken.ThrowIfCancellationRequested();

               CellStates[] commons = GetCommons(shadings);
               for (int c = 0; c < commons.Length; c++)
               {
                  switch (commons[c])
                  {
                     case Shaded:
                        if (Solution[c, w_constraint_no] == Unshaded)
                           throw new Exception("Inconsistent puzzle!");
                        if (Solution[c, w_constraint_no] != Shaded)
                        {
                           Solution[c, w_constraint_no] = Shaded;
                           // last constraint got closer
                           constraints.Add(mRowConstraints[c]);
                        }
                        break;
                     case Unshaded:
                        if (Solution[c, w_constraint_no] == Shaded)
                           throw new Exception("Inconsistent puzzle!");
                        if (Solution[c, w_constraint_no] != Unshaded)
                        {
                           Solution[c, w_constraint_no] = Unshaded;
                           // last constraint got closer
                           constraints.Add(mRowConstraints[c]);
                        }
                        break;
                     default:
                        break;
                  }
               }
            }
            else if (constraint.Type == ConstraintTypes.Row)
            {
               int h_constraint_no = constraint.Position;
               var shadings = Shadings.GetAllShadings(
                  constraint.ConstraintNums,
                  Width,
                  GetSolutionRow(h_constraint_no),
                  cancelToken);

               cancelToken.ThrowIfCancellationRequested();

               CellStates[] commons = GetCommons(shadings);
               for (int c = 0; c < commons.Length; c++)
               {
                  switch (commons[c])
                  {
                     case Shaded:
                        if (Solution[h_constraint_no, c] == Unshaded)
                           throw new Exception("Inconsistent puzzle!");
                        if (Solution[h_constraint_no, c] != Shaded)
                        {
                           Solution[h_constraint_no, c] = Shaded;
                           // last constraint got closer
                           constraints.Add(mColConstraints[c]);
                        }
                        break;
                     case Unshaded:
                        if (Solution[h_constraint_no, c] == Shaded)
                           throw new Exception("Inconsistent puzzle!");
                        if (Solution[h_constraint_no, c] != Unshaded)
                        {
                           Solution[h_constraint_no, c] = Unshaded;
                           // last constraint got closer
                           constraints.Add(mColConstraints[c]);
                        }
                        break;
                     default:
                        break;
                  }
               }
            }
            else
            {
               throw new Exception("Logic error!");
            }
         }
      }

      /*************************************************
       * Given a set of shadings, find the squares in common
       *************************************************/
      private static CellStates[] GetCommons(HashSet<CellStates[]> shadings)
      {
         if (shadings.Count == 0)
         {
            return new CellStates[0];
         }
         else if (shadings.Count == 1)
         {
            return shadings.First();
         }

         CellStates[] retVal = new CellStates[shadings.First().Length];
         for (int idx = 0; idx < retVal.Length; idx++)
         {
            CellStates? lastVal = null;
            foreach (var shading in shadings)
            {
               if (lastVal == null)
               {
                  lastVal = shading[idx];
               }
               else
               {
                  if (lastVal == shading[idx])
                  {
                     retVal[idx] = lastVal.Value;
                  }
                  else
                  {
                     retVal[idx] = Unknown;
                     break;
                  }
               }
            }
         }
         return retVal;
      }

      /*************************************************
       * Utility methods
       *************************************************/
      private CellStates[] GetSolutionCol(int col_no)
      {
         CellStates[] retVal = new CellStates[Height];
         for (int idx = 0; idx < Height; ++idx)
         {
            retVal[idx] = Solution[idx, col_no];
         }
         return retVal;
      }

      private CellStates[] GetSolutionRow(int row_no)
      {
         CellStates[] retVal = new CellStates[Width];
         for (int idx = 0; idx < Width; idx++)
         {
            retVal[idx] = Solution[row_no, idx];
         }
         return retVal;
      }

   }
}
