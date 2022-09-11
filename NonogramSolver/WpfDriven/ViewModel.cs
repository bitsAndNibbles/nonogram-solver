using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NonogramSolver.WpfDriven
{
   public class ViewModel : ViewModelBase
   {
      private CancellationTokenSource mCancellationTokenSource;
      private readonly Timer mClockRefreshTimer;
      private readonly Stopwatch mSolveStopwatch;

      public ViewModel()
      {
         SelectedPuzzle = PredefinedPuzzles.FirstOrDefault();

         mClockRefreshTimer = new Timer(RefreshClockTimes);
         mSolveStopwatch = new Stopwatch();
      }

      public List<PuzzleInput> PredefinedPuzzles
      {
         get
         {
            if (_PredefinedPuzzles == null)
            {
               _PredefinedPuzzles = new List<PuzzleInput>(new PuzzleInput[]
               {
                  PuzzleInput.Chicken,
                  PuzzleInput.Dinosaur,
                  PuzzleInput.Eagle
               });
            }

            return _PredefinedPuzzles;
         }
      } private List<PuzzleInput> _PredefinedPuzzles;

      public PuzzleInput SelectedPuzzle
      {
         get { return _SelectedPuzzle; }
         set
         {
            _SelectedPuzzle = value;
            if (value != null)
            {
               ColumnConstraints = PuzzleInput.ToConstraintsString(value.ColumnConstraints);
               RowConstraints = PuzzleInput.ToConstraintsString(value.RowConstraints);
            }

            RaisePropertyChanged();
         }
      } private PuzzleInput _SelectedPuzzle;

      public string ColumnConstraints
      {
         get { return _ColumnConstraints; }
         set
         {
            _ColumnConstraints = value;
            RaisePropertyChanged();
         }
      } private string _ColumnConstraints;

      public string RowConstraints
      {
         get { return _RowConstraints; }
         set
         {
            _RowConstraints = value;
            RaisePropertyChanged();
         }
      } private string _RowConstraints;

      public RelayCommand SolveCommand
      {
         get
         {
            if (_SolveCommand == null)
            {
               _SolveCommand = new RelayCommand(
                  async () => await SolveAsync(),
                  () => !IsSolveInProgress);
            }
            return _SolveCommand;
         }
      } private RelayCommand _SolveCommand;

      public RelayCommand CancelCommand
      {
         get
         {
            if (_CancelCommand == null)
            {
               _CancelCommand = new RelayCommand(
                  () => mCancellationTokenSource.Cancel(),
                  () => IsSolveInProgress && !mCancellationTokenSource.IsCancellationRequested);
            }
            return _CancelCommand;
         }
      } private RelayCommand _CancelCommand;

      public bool IsSolveInProgress
      {
         get { return _IsSolveInProgress; }
         set
         {
            _IsSolveInProgress = value;
            RaisePropertyChanged();
            SolveCommand.RaiseCanExecuteChanged();
         }
      } private bool _IsSolveInProgress;

      public TimeSpan SolveTime
      {
         get { return _SolveTime; }
         set
         {
            _SolveTime = value;
            RaisePropertyChanged();
         }
      } private TimeSpan _SolveTime;

      public List<CellStates[]> Solution
      {
         get { return _Solution; }
         private set
         {
            _Solution = value;
            RaisePropertyChanged();
         }
      } private List<CellStates[]> _Solution;

      private async Task SolveAsync()
      {
         mCancellationTokenSource = new CancellationTokenSource();
         IsSolveInProgress = true;
         StartTimer();

         try
         {
            var rowConstraints = PuzzleInput.FromConstraintString(RowConstraints);
            var columnConstraints = PuzzleInput.FromConstraintString(ColumnConstraints);

            var solver = new Solver(new PuzzleInput(rowConstraints, columnConstraints));
            await solver.SolveAsync(mCancellationTokenSource.Token);

            if (!mCancellationTokenSource.IsCancellationRequested)
            {
               Solution = ConvertToBindableList(solver.Solution);
            }
         }
         catch (OperationCanceledException)
         {
            // the user cancelled the solve.
         }
         catch (Exception e)
         {
            MessageBox.Show(e.ToString(), "Solve failure", MessageBoxButton.OK, MessageBoxImage.Error);
         }
         finally
         {
            StopTimer();
            IsSolveInProgress = false;
         }
      }

      private void StartTimer()
      {
         TimeSpan interval = TimeSpan.FromMilliseconds(50);
         mClockRefreshTimer.Change(interval, interval);

         mSolveStopwatch.Start();
      }

      private void StopTimer()
      {
         mClockRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
         mSolveStopwatch.Reset();
      }

      private void RefreshClockTimes(object state)
      {
         SolveTime = mSolveStopwatch.Elapsed;
      }

      private List<CellStates[]> ConvertToBindableList(CellStates[,] twoDimArray)
      {
         int numRows = twoDimArray.GetLength(0);
         int numCols = twoDimArray.GetLength(1);

         var asList = new List<CellStates[]>(numRows);

         for (int rowIndex = 0; rowIndex < numRows; ++rowIndex)
         {
            var row = new CellStates[numCols];

            for (int colIndex = 0; colIndex < numCols; ++colIndex)
            {
               row[colIndex] = twoDimArray[rowIndex, colIndex];
            }

            asList.Add(row);
         }

         return asList;
      }

   }
}
