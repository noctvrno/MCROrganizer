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

namespace MCROrganizer.Core.ViewModel
{
    public class ControlLogic
    {
        #region Data Members
        private MCROrganizer.Core.View.MainControl _userControl = null;
        #endregion

        #region Accessors
        public ObservableCollection<DraggableButton> Games { get; set; } = new ObservableCollection<DraggableButton>();

        public Dictionary<DraggableButton, Double> GamesRelativeAbscissa { get; set; } = new Dictionary<DraggableButton, Double>();
        public MCROrganizer.Core.View.MainControl MainControl => _userControl;
        #endregion

        public ControlLogic(MCROrganizer.Core.View.MainControl userControl) => _userControl = userControl;

        #region Commands
        public ICommand AddGameToChallengeRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj =>
        {
            var newGame = new DraggableButton(this);
            Point relativeLocation = newGame.TranslatePoint(new Point(0, 0), VisualTreeHelper.GetParent(newGame) as Canvas);
            Games.Add(newGame);
            newGame.ItemRelativeToParentAbscissa = relativeLocation.X + newGame.DBDataContext.Width * (Games.Count - 1);
            DraggableButton.TranslateItemHorizontally(newGame, newGame.ItemRelativeToParentAbscissa);
            GamesRelativeAbscissa.Add(newGame, newGame.ItemRelativeToParentAbscissa);
        }));
        #endregion
    }
}
