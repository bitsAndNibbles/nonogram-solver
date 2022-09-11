using System;
using System.Collections.Generic;
using System.Threading;
using static NonogramSolver.CellStates;

namespace NonogramSolver
{
   class Shadings
   {
      /*************************************************
       * Given a constraint, generate all shadings for it
       * that also match the supplied partial shading
       *************************************************/
      internal static HashSet<CellStates[]> GetAllShadings(int[] constraint,
         int squaresAvailable,
         CellStates[] partial,
         CancellationToken cancelToken)
      {
         var matchingShadings = new HashSet<CellStates[]>();
         // calculate minimum squares needed
         int minimumSquaresNeeded = constraint.Length - 1;
         foreach (int c in constraint)
         {
            minimumSquaresNeeded += c;
         }
         if (minimumSquaresNeeded > squaresAvailable)
         {
            // no fillings possible
         }
         else
         {
            // add minimum filling
            CellStates[] filling = GetMinimumDistanceFilling(constraint, minimumSquaresNeeded);
            matchingShadings.Add(filling);

            // if more than minimum filling possible, determine how many iterations to run
            int stepsRemaining = squaresAvailable - minimumSquaresNeeded;
            while ((stepsRemaining > 0)
               && !cancelToken.IsCancellationRequested)
            {
               var elementsToAdd = new HashSet<CellStates[]>();
               // for each element already in matchingShadings, create new fillings based on that element, and add them
               foreach (var shading in matchingShadings)
               {
                  var moreFillings = FromAddingInSpacesOnce(shading, constraint);
                  foreach (var someFilling in moreFillings)
                  {
                     if (MatchesPartial(someFilling, partial)) // this shouldn't work but it does!
                     {
                        elementsToAdd.Add(someFilling);
                     }
                  }
               }
               matchingShadings = elementsToAdd;
               stepsRemaining--;
            }
         }
         return matchingShadings;
      }

      /*************************************************
       * Private methods used by the above code
       *************************************************/
      private static CellStates[] GetMinimumDistanceFilling(int[] constraint, int minimumSquaresNeeded)
      {
         CellStates[] retValue = new CellStates[minimumSquaresNeeded];
         int counter = 0;
         // for each constraint
         for (int idx = 0; idx < constraint.Length; idx++)
         {
            for (int idx2 = 0; idx2 < constraint[idx]; idx2++)
            {
               retValue[counter] = Shaded;
               counter++;
            }
            if (idx < constraint.Length - 1)
            {
               retValue[counter] = Unshaded;
               counter++;
            }
         }
         return retValue;
      }

      private static HashSet<CellStates[]> FromAddingInSpacesOnce(CellStates[] filling, int[] constraint)
      {
         var retValue = new HashSet<CellStates[]>();

         // prepending an unshaded square
         CellStates[] temp = new CellStates[filling.Length + 1];
         temp[0] = Unshaded;
         Array.Copy(filling, 0, temp, 1, filling.Length);
         retValue.Add(temp);

         // appending an unshaded square
         temp = new CellStates[filling.Length + 1];
         temp[temp.Length - 1] = Unshaded;
         Array.Copy(filling, 0, temp, 0, filling.Length);
         retValue.Add(temp);

         // inserting an unshaded square after each run of shaded squares
         for (int idx = 0; idx < constraint.Length - 1; idx++)
         {
            temp = new CellStates[filling.Length + 1];
            int x = GetNthUnshadedPosAfterAShadedPos(filling, idx);
            temp[x] = Unshaded;
            Array.Copy(filling, 0, temp, 0, x);
            Array.Copy(filling, x, temp, x + 1, filling.Length - x);
            retValue.Add(temp);
         }
         return retValue;
      }

      private static int GetNthUnshadedPosAfterAShadedPos(CellStates[] str, int n)
      {
         int idx = 0;
         int countdown = n;
         while (countdown >= 0)
         {
            while (str[idx] == Unshaded)
               idx++;
            while (str[idx] == Shaded)
               idx++;
            countdown--;
         }
         return idx;
      }

      private static bool MatchesPartial(CellStates[] filling, CellStates[] partial)
      {
         if (filling.Length < partial.Length)
         {
            return true;
         }
         for (int idx = 0; idx < partial.Length; idx++)
         {
            switch (partial[idx])
            {
               case Shaded:
                  if (filling[idx] == Unshaded)
                  {
                     return false;
                  }
                  break;
               case Unshaded:
                  if (filling[idx] == Shaded)
                  {
                     return false;
                  }
                  break;
               default:
                  break;
            }
         }
         return true;
      }
   }
}
