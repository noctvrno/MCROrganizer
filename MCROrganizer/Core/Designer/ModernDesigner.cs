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
            System.Windows.Forms.OpenFileDialog openFileDialog = new()
            {
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            try
            {
                switch (elementToDesign)
                {
                    case CustomizableRunElements.BackgroundImage:
                        BackgroundImage = new BitmapImage(new(openFileDialog.FileName));
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch
            {
                MessageBox.Show("Could not load the image file. Please try again with a different image.", PathUtils.Caption, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
