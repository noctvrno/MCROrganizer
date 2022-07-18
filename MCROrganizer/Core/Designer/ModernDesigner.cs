using MCROrganizer.Core.CustomControls;
using MCROrganizer.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MCROrganizer.Core.Designer
{
    public class ModernDesigner : IDesigner
    {
        private DraggableButtonDataContext _runData = null;
        public DraggableButtonDataContext RunData => _runData;

        private ImageSource _backgroundImage = null;
        public ImageSource BackgroundImage
        {
            get => _backgroundImage;
            set
            {
                _backgroundImage = value;
                NotifyPropertyChanged(nameof(BackgroundImage));
            }
        }

        public ModernDesigner(DraggableButtonDataContext runData, RunState runState)
        {
            _runData = runData;
            BackgroundImage = new BitmapImage(new(Path.Combine(PathUtils.ImagePath, $"{runState}DefaultBGImage.png")));
        }

        public void Design(CustomizableRunElements elementToDesign)
        {
            // Turn this into a switch if we ever have multiple Elements to deal with for the ModernDesigner.
            if (elementToDesign != CustomizableRunElements.BackgroundImage)
                return;

            if (!OpenFileDialogUtils.TryGetImage(out BitmapImage backgroundImage))
                return;

            BackgroundImage = backgroundImage;

            // Don't try changing to Double.NaN for Auto scale. This will break further computations.
            RunData.Width = BackgroundImage.Width;
            RunData.Height = BackgroundImage.Height;

            // Update runs in order to recompute their position on the screen.
            RunData.Control.DBParent.UpdateRuns();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
