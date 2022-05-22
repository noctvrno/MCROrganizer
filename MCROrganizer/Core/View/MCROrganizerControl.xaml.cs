using System;
using System.Collections.Generic;
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
using MCROrganizer.Core.CustomControls;

namespace MCROrganizer.Core.View
{
    /// <summary>
    /// Interaction logic for MCROrganizerControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        private ViewModel.ControlLogic _dataContext = null;
        public MainControl()
        {
            DataContext = _dataContext = new ViewModel.ControlLogic(this);
            InitializeComponent();
        }

        private void OnUserControlChanged(object sender, SizeChangedEventArgs e)
        {
            if (_dataContext == null)
                return;

            // The ItemsPanelTemplate of our ItemsControl where each run is placed is a Canvas because we need the ability to drag the runs.
            // Canvases are not stretchable and we cannot change this.
            // Whenever the window/main control resizes, then we'll need to update width of the ItemsControl and recompute and replace everything on the screen.
            // Even if the ItemsControl was stretchable we couldn't have possible triggered the recomputation of abscissaCases in a clean way.
            _dataContext.ItemsControlWidth = e.NewSize.Width;
        }
    }
}