using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NonogramSolver.WpfDriven
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
      }

      protected override void OnInitialized(EventArgs e)
      {
         base.OnInitialized(e);

         DataContextChanged += HandleDataContextChange;
         if (DataContext != null)
         {
            HandleDataContextChange(this, new DependencyPropertyChangedEventArgs(DataContextProperty, null, DataContext));
         }
      }

      private void HandleDataContextChange(object sender, DependencyPropertyChangedEventArgs e)
      {
         var oldVm = e.OldValue as ViewModel;
         var newVm = e.NewValue as ViewModel;

         if (oldVm != null)
         {
            oldVm.PropertyChanged -= HandleVMPropChange;

         }

         if (newVm != null)
         {
            newVm.PropertyChanged += HandleVMPropChange;
         }
      }

      private void HandleVMPropChange(object sender, PropertyChangedEventArgs e)
      {
         if ((sender is ViewModel)
            && (e.PropertyName == "Solution"))
         {
            var vm = sender as ViewModel;
            var solution = vm.Solution;

            DisplaySolution(solution);
         }
      }

      private void DisplaySolution(List<CellStates[]> solution)
      {
         ClearSolutionGrid();

         var firstRow = solution.FirstOrDefault();
         if (firstRow == null)
         {
            // there is no solution for some reason
            return;
         }

         int numRows = solution.Count;
         int numCols = firstRow.Length;

         PrepareSolutionGrid(numRows, numCols);

         for (int rowIndex = 0; rowIndex < numRows; ++rowIndex)
         {
            var row = solution[rowIndex];
            for (int colIndex = 0; colIndex < numCols; ++colIndex)
            {
               var cell = row[colIndex];

               Brush backgroundFill = GetBackgroundFill(cell);

               var cellContent = new Border()
               {
                  Background = backgroundFill,
                  Width = 1,
                  Height = 1
               };

               Grid.SetRow(cellContent, rowIndex);
               Grid.SetColumn(cellContent, colIndex);

               SolutionGrid.Children.Add(cellContent);
            }
         }
      }

      private void ClearSolutionGrid()
      {
         SolutionGrid.Children.Clear();
         SolutionGrid.RowDefinitions.Clear();
         SolutionGrid.ColumnDefinitions.Clear();
      }

      private void PrepareSolutionGrid(int numRows, int numCols)
      {
         for (int i = 0; i < numRows; ++i)
         {
            SolutionGrid.RowDefinitions.Add(new RowDefinition());
         }
         for (int i = 0; i < numCols; ++i)
         {
            SolutionGrid.ColumnDefinitions.Add(new ColumnDefinition());
         }
      }

      private static Brush GetBackgroundFill(CellStates cellState)
      {
         Brush backgroundFill;

         switch (cellState)
         {
            case CellStates.Shaded:
               backgroundFill = Brushes.Black;
               break;
            case CellStates.Unknown:
            case 0: // the default state when allocated
               backgroundFill = Brushes.Red;
               break;
            case CellStates.Unshaded:
               backgroundFill = Brushes.Beige;
               break;
            default:
               throw new Exception($"Unexpected cell state {cellState}");
         }

         return backgroundFill;
      }

   }
}
