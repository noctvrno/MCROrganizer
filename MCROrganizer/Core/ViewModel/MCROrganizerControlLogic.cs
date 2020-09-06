using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using MCROrganizer.Core.Commands;

namespace MCROrganizer.Core.ViewModel
{
    public class ControlLogic
    {
        #region Data Members
        private ObservableCollection<Button> challengeRunGames = new ObservableCollection<Button>()
        {
            new Button() { Content = "First Button"},
            new Button() { Content = "Second Button"}
        };
        #endregion

        #region Commands
        public ICommand AddGameToChallengeRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj =>
        {
            ChallengeRunGames.Add(new Button() { Content = "Button added dynamically" });
        }));
        #endregion

        #region Accessors
        public ObservableCollection<Button> ChallengeRunGames
        {
            get => challengeRunGames;
            set => challengeRunGames = value;
        }
        #endregion
    }
}
