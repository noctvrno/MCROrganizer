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
        public Dictionary<DraggableButton, Double> AbscissaByRun { get; set; } = new Dictionary<DraggableButton, Double>();
        public ObservableCollection<DraggableButton> Runs { get; set; } = new ObservableCollection<DraggableButton>();
        public MainControl MainControl => _userControl;
        #endregion

        #region Initialization
        public ControlLogic(MainControl userControl) => _userControl = userControl;
        #endregion

        #region Commands
        public ICommand AddGameToChallengeRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj =>
        {
            var newGame = new DraggableButton(this);
            Runs.Add(newGame);
            Point relativeLocation = newGame.TranslatePoint(new Point(0, 0), VisualTreeHelper.GetParent(newGame) as Canvas);
            Double gameSpacing = 10.0;
            Double newGameAbscissaValue = relativeLocation.X + (newGame.DBDataContext.Width + gameSpacing) * (Runs.Count - 1);
            // Translate the item accordingly and shift the pivot point to the middle of the button.
            TranslateItemHorizontally(newGame, newGameAbscissaValue);
            AbscissaByRun.Add(newGame, newGameAbscissaValue);
        }));
        #endregion

        #region Helper methods
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
            }
        }
        #endregion
    }
}
