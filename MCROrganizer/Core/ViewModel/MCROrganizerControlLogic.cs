using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MCROrganizer.Core.Commands;
using MCROrganizer.Core.CustomControls;
using MCROrganizer.Core.Utils;
using MCROrganizer.Core.View;

namespace MCROrganizer.Core.ViewModel
{
    public enum RunState
    {
        Pending,
        InProgress,
        Finished
    }

    public class ControlLogic : UserControlDataContext
    {
        #region Data Members
        private Double _controlWidth = 400.0;
        private Thickness _itemsControlMargins = new Thickness(20.0, 20.0, 20.0, 10.0);
        private Double _itemsControlWidth = 0.0;
        private Double _spacingBetweenRuns = 20.0;
        private Dictionary<DraggableButton, Double> _abscissaByRun = null;
        private Dictionary<Int32, List<Double>> _abscissaByNumberOfRunsCases = null;
        private ObservableCollection<DraggableButton> _runs = null;
        private MainControl _userControl = null;
        #endregion

        #region Accessors
        public Double ControlWidth
        {
            get => _controlWidth;
            set => _controlWidth = value;
        }
        public Thickness ItemsControlMargins => _itemsControlMargins;
        public Double ItemsControlWidth => _itemsControlWidth;
        public ObservableCollection<DraggableButton> Runs => _runs;
        public Dictionary<DraggableButton, Double> AbscissaByRun => _abscissaByRun;
        public LinkedList<(Double abscissa, Int32 vacantIndex)> AbscissasInQueue { get; set; }
        public MainControl MainControl => _userControl;
        #endregion

        #region Initialization
        public ControlLogic(MainControl userControl)
        {
            _userControl = userControl;
            InitializeDefaultRuns(ref _runs, ref _abscissaByRun);
            AbscissasInQueue = new LinkedList<(Double abscissa, Int32 vacantIndex)>();
            ComputeAbscissaCases(_abscissaByNumberOfRunsCases);
        }
        #endregion

        #region Commands
        public ICommand AddRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj => AddRun()));
        #endregion

        #region Functionality
        // This method translates the control horizontally.
        public void TranslateItemHorizontally(DraggableButton itemToTranslate, Double abscissaValue)
        {
            Canvas.SetLeft(itemToTranslate, abscissaValue);
        }

        // This method moves the collidedItem to the draggedItem position and updates the dictionary according to the swap (also swaps the entries in the ItemsSource).
        public void SwapDraggedItemOnCollision(DraggableButton draggedItem, DraggableButton collidedItem)
        {
            // Update the dictionary so that the draggedItem has the collidedItem's position.
            if (AbscissaByRun.TryGetValue(draggedItem, out Double draggedItemAbscissaValue) && AbscissaByRun.TryGetValue(collidedItem, out Double collidedItemAbscissaValue))
            {
                // Physically move the collideditem to the draggedItem standard position.
                TranslateItemHorizontally(collidedItem, draggedItemAbscissaValue);

                // Swap the values of the draggedItem and the collidedItem.
                AbscissaByRun[draggedItem] = collidedItemAbscissaValue;
                AbscissaByRun[collidedItem] = draggedItemAbscissaValue;

                // Swap the items in the observable collection as well. Due to the nature of this data structure, we cannot simply do a swap here.
                // The Move method actually removes the entry and then re-inserts it (a bit of overhead).
                _runs.Move(_runs.IndexOf(draggedItem), _runs.IndexOf(collidedItem));

                SetAllRunsAsPending();
            }
        }

        public void SetRunAsCurrent(DraggableButton selectedRun)
        {
            Int32 selectedRunIndex = _runs.IndexOf(selectedRun);
            selectedRun.DBDataContext.RunState = RunState.InProgress;

            for (Int32 runIndex = 0; runIndex < _runs.Count; ++runIndex)
            {
                // Skip the selected run (already handled).
                if (runIndex == selectedRunIndex)
                    continue;

                // Runs to the left of the current one will be considered finished and the ones to left are considered pending.
                _runs[runIndex].DBDataContext.RunState = runIndex < selectedRunIndex ? RunState.Finished : RunState.Pending;
            }
        }

        public void SetAllRunsAsPending()
        {
            // Firstly, find a run that is not pending.
            if (_runs.Any(x => x.DBDataContext.RunState != RunState.Pending))
            {
                // Secondly, if we found one, then go through all of them and set them as pending until the user decides on a current run.
                foreach (var run in _runs.Select(x => x.DBDataContext))
                {
                    run.RunState = RunState.Pending;
                }
            }
        }

        private void AddRun(DraggableButton newRun = null)
        {
            if (newRun == null)
                newRun = new DraggableButton(this);

            Double newRunAbscissaValue = 0.0;
            Point relativeLocation = newRun.TranslatePoint(new Point(0, 0), VisualTreeHelper.GetParent(newRun) as Canvas);

            if (AbscissasInQueue.Count > 0)
            {
                var queuedAbscissa = AbscissasInQueue.First();
                _runs.Insert(queuedAbscissa.vacantIndex, newRun);
                newRunAbscissaValue = queuedAbscissa.abscissa;
                AbscissasInQueue.RemoveFirst();
            }
            else
            {
                _runs.Add(newRun);
                newRunAbscissaValue = relativeLocation.X + (newRun.DBDataContext.Width + _spacingBetweenRuns) * (_runs.Count - 1);
            }

            // Translate the item accordingly and shift the pivot point to the middle of the button.
            TranslateItemHorizontally(newRun, newRunAbscissaValue);
            _abscissaByRun.Add(newRun, newRunAbscissaValue);
        }

        public void RemoveRun(DraggableButton deletedRun)
        {
            // Place the deleted position and its index in the linked list and sort it.
            AbscissasInQueue.AddLast((AbscissaByRun[deletedRun], _runs.IndexOf(deletedRun)));
            AbscissasInQueue = new LinkedList<(Double abscissa, Int32 vacantIndex)>(AbscissasInQueue.OrderBy(x => x.abscissa));

            // Secondly, Remove the objects from the data structures.
            _runs.Remove(deletedRun);
            AbscissaByRun.Remove(deletedRun);
        }

        private void InitializeDefaultRuns(ref ObservableCollection<DraggableButton> runs, ref Dictionary<DraggableButton, double> abscissaByRun)
        {
            runs = new ObservableCollection<DraggableButton>()
            {
                new DraggableButton(this),
                new DraggableButton(this)
            };

            abscissaByRun = new Dictionary<DraggableButton, Double>();

            // Compute the actual width of the ItemsControl (where the buttons will be placed).
            _itemsControlWidth = _controlWidth - _itemsControlMargins.Left - _itemsControlMargins.Right;
            NotifyPropertyChanged("ItemsControlWidth");

            Double runWidth = runs.FirstOrDefault().DBDataContext.Width;
            Double nextPivotPoint = _spacingBetweenRuns + runWidth;
            Double startAbscissa = _itemsControlWidth / 2.0 - _spacingBetweenRuns / 2.0 - runWidth;

            // The AbscissaByRun dictionary represents the position of each button relative to the position of the left-most button.
            // Because we are working with an ItemsControl that covers a wider area, then we'll have to place things a bit differently.
            // x = 0 position will actually be relative to the start of the ItemsControl.
            foreach (var run in runs)
            {
                abscissaByRun.Add(run, nextPivotPoint * abscissaByRun.Count);
                TranslateItemHorizontally(run, startAbscissa + nextPivotPoint * (abscissaByRun.Count - 1));
            }
        }

        private void ComputeAbscissaCases(Dictionary<Int32, List<Double>> abscissaByNumberOfRunsCases)
        {
            abscissaByNumberOfRunsCases = new Dictionary<Int32, List<Double>>();

            Double runWidth = _runs.FirstOrDefault().DBDataContext.Width;
            Double nextPivotPoint = _spacingBetweenRuns + runWidth;
            Int32 maximumNumberOfRuns = (Int32)Math.Floor(_itemsControlWidth / (runWidth + _spacingBetweenRuns));

            for (Int32 iRunCase = _runs.Count; iRunCase < maximumNumberOfRuns; ++iRunCase)
            {
                // Have a List of tuples here that holds the abscissas relative to the group as the first item and the abscissas relative to the ItemsControl as the second item.
                List<Double> abscissas = new List<Double>();
                for (Int32 iRun = 0; iRun < iRunCase; ++iRun)
                {
                    abscissas.Add(nextPivotPoint * abscissas.Count);
                }

                abscissaByNumberOfRunsCases.Add(iRunCase, abscissas);
            }
        }
        #endregion
    }
}
