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
using MCROrganizer.Core.View;

namespace MCROrganizer.Core.ViewModel
{
    public enum RunState
    {
        Pending,
        InProgress,
        Finished
    }

    public class ControlLogic
    {
        #region Data Members
        private MainControl _userControl = null;
        #endregion

        #region Accessors
        public Dictionary<DraggableButton, Double> AbscissaByRun { get; }
        public ObservableCollection<DraggableButton> Runs { get; }
        public LinkedList<(Double abscissa, Int32 vacantIndex)> AbscissasInQueue { get; set; }
        public MainControl MainControl => _userControl;
        #endregion

        #region Initialization
        public ControlLogic(MainControl userControl)
        {
            _userControl = userControl;
            AbscissaByRun = new Dictionary<DraggableButton, Double>();
            Runs = new ObservableCollection<DraggableButton>();
            AbscissasInQueue = new LinkedList<(Double abscissa, Int32 vacantIndex)>();
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
                Runs.Move(Runs.IndexOf(draggedItem), Runs.IndexOf(collidedItem));

                SetAllRunsAsPending();
            }
        }

        public void SetRunAsCurrent(DraggableButton selectedRun)
        {
            Int32 selectedRunIndex = Runs.IndexOf(selectedRun);
            selectedRun.DBDataContext.RunState = RunState.InProgress;

            for (Int32 runIndex = 0; runIndex < Runs.Count; ++runIndex)
            {
                // Skip the selected run (already handled).
                if (runIndex == selectedRunIndex)
                    continue;

                // Runs to the left of the current one will be considered finished and the ones to left are considered pending.
                Runs[runIndex].DBDataContext.RunState = runIndex < selectedRunIndex ? RunState.Finished : RunState.Pending;
            }
        }

        public void SetAllRunsAsPending()
        {
            // Firstly, find a run that is not pending.
            if (Runs.Any(x => x.DBDataContext.RunState != RunState.Pending))
            {
                // Secondly, if we found one, then go through all of them and set them as pending until the user decides on a current run.
                foreach (var run in Runs.Select(x => x.DBDataContext))
                {
                    run.RunState = RunState.Pending;
                }
            }
        }

        private void AddRun()
        {
            var newRun = new DraggableButton(this);
            Double newRunAbscissaValue = 0.0;
            Double spacing = 10.0;
            Point relativeLocation = newRun.TranslatePoint(new Point(0, 0), VisualTreeHelper.GetParent(newRun) as Canvas);

            if (AbscissasInQueue.Count > 0)
            {
                var queuedAbscissa = AbscissasInQueue.First();
                Runs.Insert(queuedAbscissa.vacantIndex, newRun);
                newRunAbscissaValue = queuedAbscissa.abscissa;
                AbscissasInQueue.RemoveFirst();
            }
            else
            {
                Runs.Add(newRun);
                newRunAbscissaValue = relativeLocation.X + (newRun.DBDataContext.Width + spacing) * (Runs.Count - 1);
            }

            // Translate the item accordingly and shift the pivot point to the middle of the button.
            TranslateItemHorizontally(newRun, newRunAbscissaValue);
            AbscissaByRun.Add(newRun, newRunAbscissaValue);
        }

        public void RemoveRun(DraggableButton deletedRun)
        {
            // Place the deleted position and its index in the linked list and sort it.
            AbscissasInQueue.AddLast((AbscissaByRun[deletedRun], Runs.IndexOf(deletedRun)));
            AbscissasInQueue = new LinkedList<(Double abscissa, Int32 vacantIndex)>(AbscissasInQueue.OrderBy(x => x.abscissa));

            // Secondly, Remove the objects from the data structures.
            Runs.Remove(deletedRun);
            AbscissaByRun.Remove(deletedRun);
        }
        #endregion
    }
}
