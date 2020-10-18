using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MCROrganizer.Core.Commands;
using MCROrganizer.Core.CustomControls;
using MCROrganizer.Core.View;

namespace MCROrganizer.Core.ViewModel
{
    public class ControlLogic
    {
        #region Data Members
        private MainControl _userControl = null;
        #endregion

        #region Accessors
        public Dictionary<DraggableButton, Double> GamesRelativeAbscissa { get; set; } = new Dictionary<DraggableButton, Double>();
        public List<(Double abscissa, Boolean vacancy)> StandardPositionVacancy { get; set; } = new List<(Double abscissa, Boolean vacancy)>();
        public ObservableCollection<DraggableButton> Games { get; set; } = new ObservableCollection<DraggableButton>();
        public MainControl MainControl => _userControl;
        #endregion

        #region Initialization
        public ControlLogic(MainControl userControl) => _userControl = userControl;
        #endregion

        #region Commands
        public ICommand AddGameToChallengeRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj =>
        {
            var newGame = new DraggableButton(this);
            Point relativeLocation = newGame.TranslatePoint(new Point(0, 0), VisualTreeHelper.GetParent(newGame) as Canvas);
            Double gameSpacing = 10.0;
            Games.Add(newGame);
            // Translate the item accordingly and shift the pivot point to the middle of the button.
            newGame.TranslateItemHorizontally(newGame, relativeLocation.X + (newGame.DBDataContext.Width + gameSpacing) * (Games.Count - 1));
            GamesRelativeAbscissa.Add(newGame, newGame.ItemRelativeToParentAbscissa);
            StandardPositionVacancy.Add((newGame.ItemRelativeToParentAbscissa, false));
        }));
        #endregion
    }
}
