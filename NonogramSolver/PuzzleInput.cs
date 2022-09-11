using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonogramSolver
{
   public class PuzzleInput
   {
      public int[][] RowConstraints { get; }

      public int[][] ColumnConstraints { get; }

      public string Name { get; }

      internal static PuzzleInput Chicken
      {
         get
         {
            return new PuzzleInput(
               name: "Chicken",
               rowConstraints: new int[][]
               {
                  new int[]{ 2 },
                  new int[]{ 1, 1 },
                  new int[]{ 4 },
                  new int[]{ 2, 1 },
                  new int[]{ 3, 1 },
                  new int[]{ 8 },
                  new int[]{ 8 },
                  new int[]{ 7 },
                  new int[]{ 5 },
                  new int[]{ 3 }
               },
               columnConstraints: new int[][]
               {
                  new int[]{ 1 },
                  new int[]{ 2 },
                  new int[]{ 1, 6},
                  new int[]{ 9 },
                  new int[]{ 6 },
                  new int[]{ 5 },
                  new int[]{ 5 },
                  new int[]{ 4 },
                  new int[]{ 3 },
                  new int[]{ 4 }
               });
         }
      }

      internal static PuzzleInput Eagle
      {
         get
         {
            return new PuzzleInput(
               name: "Eagle",
               rowConstraints: FromConstraintString(
                  "3 2, 3 4, 4 5, 5 7, 6 7, 8 6, 3 2 7, 2 3 8, 1 1 2 7, 6 5, 1 3 6, 3 6, 1 6, 2 7 2, 1 1 5 3, 2 1 6 1 5, 2 17, 3 16, 4 8, 3 3 3, 3 3 3, 4 2 3 3, 3 2 1 1 4, 4 3 2 3 5, 3 2 1 5 5, 7 13, 26, 17, 11, 9"),
               columnConstraints: FromConstraintString(
                  "1 5, 2 7, 3 7, 3 7, 4 6, 6 5, 6 4, 4 2 1 5, 2 3 4 2 5, 2 1 5 2 4 5, 1 5 3 3 5, 2 4 1 3 2 4, 2 1 6 1 4, 2 9 1 4, 16 2 4, 20 4, 12 6 1 3, 11 4 3, 10 3 3, 7 4 4, 6 6 5, 4 5 6, 3 4 4, 2 3 3, 1 2 2, 4, 5, 6, 6, 6"));
         }
      }

      internal static PuzzleInput Dinosaur
      {
         get
         {
            return new PuzzleInput(
               name: "Dinosaur",
               rowConstraints: FromConstraintString(
                  "0, 5, 3 2, 4 3, 1 1 2 3, 5 2 2, 1 1 3 1 3, 3 2 1, 1 2 3 1, 3 1 3, 1 1 1, 1 1, 1 1, 1"),
               columnConstraints: FromConstraintString(
                  "3, 1 1 1, 4 1, 3 2 1, 5 2, 1 1 2, 1 1 2 1, 9, 5 1 1, 1 5, 3 1 1, 1 5, 2, 1, 1"));
         }
      }

      internal PuzzleInput(int[][] rowConstraints, int[][] columnConstraints, string name = "")
      {
         RowConstraints = rowConstraints;
         ColumnConstraints = columnConstraints;
         Name = name;
      }

      internal static int[][] FromConstraintString(string constraintString)
      {
         var lineSeparator = new string[] { ", ", "," };
         var fillRuleSeparator = new string[] { " " };

         var constraintStrings = constraintString.Split(lineSeparator, StringSplitOptions.None);

         int[][] input = new int[constraintStrings.Length][];

         for (int i = 0; i < input.Length; ++i)
         {
            var lineConstraintsAsString = constraintStrings[i];
            var ruleStrings = lineConstraintsAsString.Split(fillRuleSeparator, StringSplitOptions.RemoveEmptyEntries);

            input[i] = ruleStrings.Select(rule => int.Parse(rule)).ToArray();
         }

         return input;
      }

      internal static string ToConstraintsString(int[][] lineRules)
      {
         var withInnerStrings = lineRules.Select(constraints => string.Join(" ", constraints));

         var withOuterStrings = string.Join(", ", withInnerStrings);

         return withOuterStrings;
      }



   //////////////////////////////////////////////////////////////////////////////////////
   //
   //
   // Cat puzzle
   // ---------------------------------------------
   //
   //////////////////////////////////////////////////////////////////////////////////////
   // private static final int WIDTH = 20;
   // private static final int HEIGHT = 30;
   // private static final int[][] H_CONSTRAINTS = { { 1, 3, 1 }, { 3, 1, 1, 3, 1 }, { 1, 1, 7, 1, 1, 4 }, { 3, 1, 1,
   // 1, 1, 3, 1, 2 }, { 1, 1, 7, 1, 1, 4 }, { 3, 7, 5, 2 }, { 3 }, { 1, 1, 5 }, { 2, 2, 7 }, { 8, 3, 3 }, { 1, 2, 3,
   // 2, 2, 2 }, { 1, 2, 3, 3, 2, 2 }, { 9, 5, 2 }, { 1, 4, 2, 3, 2 }, { 2, 2, 3 }, { 6, 3 }, { 2, 1, 3 }, { 1, 3, 2 },
   // { 7, 2 }, { 9, 3 }, { 10, 3 }, { 11, 3 }, { 11, 2 }, { 2, 2, 6, 2 }, { 2, 2, 2, 5 }, { 2, 2, 1, 6 }, { 2, 2, 1, 5
   // }, { 1, 2, 6 }, { 1, 4, 6 }, { 1, 3, 6 } };
   // private static final int[][] W_CONSTRAINTS = { { 6 }, { 2, 1, 2, 3 }, { 6, 4, 2 }, { 2, 2, 1 }, {
   // 4, 7, 1, 5, 2 }, { 1, 2, 5, 2, 10 }, { 4, 1, 2, 2, 9, 2 }, { 1, 2, 4, 2, 6, 2 }, { 6, 7, 14 }, { 1, 1, 2, 7, 12
   // }, { 6, 5, 1 }, { 3, 8, 2 }, { 5, 5, 6, 3 }, { 2, 1, 1, 3, 3, 4, 5 }, { 5, 2, 2, 3, 5 }, { 1, 2, 4, 4, 8 }, { 4,
   // 2, 3, 6, 6 }, { 1, 1, 3, 3, 3, 4 }, { 5, 8, 6 }, { 4, 6, 4 } };
   //
   //////////////////////////////////////////////////////////////////////////////////////
   //
   // Online puzzle - Two Cats (takes a while to run)
   // ---------------------------------------------
   //
   //////////////////////////////////////////////////////////////////////////////////////
   // private static final int WIDTH = 50;
   // private static final int HEIGHT = 48;
   // private static final int[][] H_CONSTRAINTS =
   // {{1},{3,2},{4,3,3},{15},{15},{14,1},{12,2},{12,3},{13,4},{14,5},{14,7,2},{13,13},{11,14},{9,14},{7,13},{1,5,12,1},{1,8,10,1},
   // {1,10,9,2},{2,11,9,3},{2,11,8,3},{3,11,7,4},{4,10,6,5},{5,8,7,6},{6,7,8,7},{8,2,7,8,8},{7,5,8,8,9},{7,6,8,11,11},{7,16,13,10},{7,15,11,3,9},
   // {7,14,1,10,3,9},{7,13,3,14,9},{6,17,14,9},{6,17,14,9},{5,17,14,10},{6,15,1,12,3,3},{7,12,3,10,3,3},{2,2,10,1,8,3,3},{2,3,4,2,2,4},{2,3,5,4,2,4},{5,5},{5,9,3,3},
   // {5,11,3,3},{2,3,5,3,9,3,3},{2,3,5,2,4,3,3},{2,3,5,1,3,4,3,3},{2,3,5,4,4,3,3},{2,3,5,4,4,3,3},{2,3,5,3,4,3,3}};
   // private static final int[][] W_CONSTRAINTS =
   // {{24,6},{21,6},{16},{15},{14},{10,2},{7,1},{1,2},{9,3,6},{10,3,6},{12,2,6},{13},{12},{12,6},{18},{3,19},{5,21},{3,7,3,23},{13,5,13,4},{15,21,1,2,3},{14,21,1,3,2},
   // {28,5,2,1,1,1},{26,5,3,4},{26,5,1,7},{24,1,2,2,7},{22,3,2,1,4,2},{10,7,11,3},{9,3,5,13,3},{8,5,17,3,4},{5,28,3,4},{5,30,4,4},{4,37,4},{37},{34},{15,14},
   // {11,2,8},{10,2,6},{8,8},{5,6},{4,1,3,3,8},{4,2,4,8},{3,12,8},{10},{11},{12},{13},{14},{19,8},{20,8},{22,8}};
   //
   //////////////////////////////////////////////////////////////////////////////////////
   //
   // Jonathan puzzle 1
   // ---------------------------------------------
   //
   //////////////////////////////////////////////////////////////////////////////////////
   // private static final int WIDTH = 15;
   // private static final int HEIGHT = 13;
   // private static final int[][] H_CONSTRAINTS =
   // {{5},{3,2},{4,3},{1,1,2,3},{5,2,2},{1,1,3,1,3},{3,2,1},{1,2,3,1},{3,1,3},{1,1,1},{1,1},{1,1},{1}};
   // private static final int[][] W_CONSTRAINTS =
   // {{3},{1,1,1},{4,1},{3,2,1},{5,2},{1,1,2},{1,1,2,1},{9},{5,1,1},{1,5},{3,1,1},{1,5},{2},{1},{1}};
   //
   //////////////////////////////////////////////////////////////////////////////////////
   //
   // Jonathan puzzle 2
   // ---------------------------------------------
   //
   //////////////////////////////////////////////////////////////////////////////////////
   // private static final int WIDTH = 25;
   // private static final int HEIGHT = 20;
   // private static final int[][] H_CONSTRAINTS =
   // {{4},{1,5,3},{8,8},{14},{2,2,3},{1,2,2,2},{3,2,3,2},{3,4,2,2},{5,3,2,5},{3,2,1,3,4},{5,5,3,6},{6,4,3,3},{3,2,2,4,2,3},{3,2,7,1},{3,2,2},{10},{4,3},{6,4},{5},{1}};
   // private static final int[][] W_CONSTRAINTS =
   // {{2},{1,1,1},{3,5},{3,6},{4,5},{11},{4,3,2},{2,5,2},{3,3,2},{3,1,1,2,2,2},{2,2,4,5},{2,1,2,4,4},{3,3,2,1,4},{4,3,2,1,2},{4,2,2,2,1,3},{4,3,2,1},{2,2,3,2,1},{1,2,1,1,1},{2,3,3},{10},{4,2},{5,1},{5,1},{3},{1}};
   //
   //////////////////////////////////////////////////////////////////////////////////////

}
}
