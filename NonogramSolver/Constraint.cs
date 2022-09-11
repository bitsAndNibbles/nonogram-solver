using System;
using System.Linq;

namespace NonogramSolver
{
   class Constraint : IComparable<Constraint>
   {
      internal int[] ConstraintNums { get; }

      internal ConstraintTypes Type { get; }

      internal int Position { get; }

      internal int TotalOfConstraintNumsAndLength { get; }

      internal Constraint(int[] constraintNums, ConstraintTypes type, int position)
      {
         if (constraintNums == null)
         {
            throw new ArgumentNullException("constraintNums");
         }
         if (!constraintNums.Any())
         {
            throw new ArgumentException("must not be empty", "constraintNums");
         }

         ConstraintNums = constraintNums;
         Type = type;
         Position = position;

         TotalOfConstraintNumsAndLength = constraintNums.Length - 1;
         TotalOfConstraintNumsAndLength += constraintNums.Sum();
      }

      public int CompareTo(Constraint other)
      {
         // compare to results are intentionally reversed so that sorting is in descending order
         //return other.TotalOfConstraintNumsAndLength.CompareTo(TotalOfConstraintNumsAndLength);

         if (other.TotalOfConstraintNumsAndLength > TotalOfConstraintNumsAndLength)
         {
            return 1;
         }
         else if (other.TotalOfConstraintNumsAndLength < TotalOfConstraintNumsAndLength)
         {
            return -1;
         }
         else if (other.Type != Type)
         {
            return other.Type < Type ? 1 : -1;
         }
         else // total of constraints and type are identical
         {
            return other.Position.CompareTo(Position);
         }
      }

      public override string ToString()
      {
         var constraintNumsString = string.Join(", ", ConstraintNums);
         return $"Constraint [constraintNums={constraintNumsString}, type={Type}, position={Position}]";
      }

   }
}
