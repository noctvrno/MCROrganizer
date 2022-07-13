using MCROrganizer.Core.CustomControls;
using MCROrganizer.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MCROrganizer.Core.Designer
{
    public class ClassicDesigner : IDesigner, INotifyPropertyChanged
    {
        private DraggableButtonDataContext _runData = null;
        public DraggableButtonDataContext RunData => _runData;

        private SolidColorBrush _borderColor = null;
        public SolidColorBrush BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                NotifyPropertyChanged(nameof(BorderColor));
            }
        }

        private Double _borderThickness = 2.0;
        public Double BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;
                NotifyPropertyChanged(nameof(BorderThickness));
            }
        }

        private SolidColorBrush _backgroundColor = null;
        public SolidColorBrush BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                NotifyPropertyChanged(nameof(BackgroundColor));
            }
        }

        private SolidColorBrush _fontColor = null;
        public SolidColorBrush FontColor
        {
            get => _fontColor;
            set
            {
                _fontColor = value;
                NotifyPropertyChanged(nameof(FontColor));
            }
        }

        public ClassicDesigner(DraggableButtonDataContext runData, RunState runState)
        {
            _runData = runData;
            BorderColor = runState switch
            {
                RunState.Pending => new SolidColorBrush(Colors.Red),
                RunState.InProgress => new SolidColorBrush(Colors.Yellow),
                RunState.Finished => new SolidColorBrush(Colors.Green),
                RunState _ => throw new NotSupportedException("Unreachable")
            };

            BackgroundColor = new SolidColorBrush(Colors.Transparent);
            FontColor = new SolidColorBrush(Colors.Black);
        }

        public void Design(CustomizableRunElements elementToDesign)
        {
            Int32 customColor = 0;
            System.Drawing.Color selectedColor = new();

            SolidColorBrush choiceColorBrush = elementToDesign switch
            {
                CustomizableRunElements.BorderColor => BorderColor,
                CustomizableRunElements.BackgroundColor => BackgroundColor,
                CustomizableRunElements.FontColor => FontColor,
                CustomizableRunElements _ => throw new NotSupportedException()
            };

            customColor = BitConverter.ToInt32(new byte[] { choiceColorBrush.Color.R, choiceColorBrush.Color.G, choiceColorBrush.Color.B, 0 }, 0);
            selectedColor = System.Drawing.Color.FromArgb(choiceColorBrush.Color.A, choiceColorBrush.Color.R, choiceColorBrush.Color.G, choiceColorBrush.Color.B);

            var colorDialog = new System.Windows.Forms.ColorDialog()
            {
                AllowFullOpen = true,
                FullOpen = true,
                Color = selectedColor,
                CustomColors = new Int32[] { customColor }
            };

            if (colorDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            switch (elementToDesign)
            {
                case CustomizableRunElements.BorderColor:
                    BorderColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                    break;
                case CustomizableRunElements.BackgroundColor:
                    BackgroundColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                    break;
                case CustomizableRunElements.FontColor:
                    FontColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
