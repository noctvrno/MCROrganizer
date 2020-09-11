using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using MCROrganizer.Core.Commands;
using MCROrganizer.Core.CustomControls;

namespace MCROrganizer.Core.ViewModel
{
    public class ControlLogic
    {
        #region Data Members
        private ObservableCollection<DraggableButton> _challengeRunGames = new ObservableCollection<DraggableButton>();
        #endregion

        #region Accessors
        public ObservableCollection<DraggableButton> ChallengeRunGames
        {
            get => _challengeRunGames;
            set => _challengeRunGames = value;
        }
        #endregion

        #region Commands
        public ICommand AddGameToChallengeRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj =>
        {
            ChallengeRunGames.Add(new DraggableButton());
        }));
        #endregion
    }
}
