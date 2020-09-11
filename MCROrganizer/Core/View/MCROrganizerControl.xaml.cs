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

namespace MCROrganizer.Core.View
{
    /// <summary>
    /// Interaction logic for MCROrganizerControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        private Control draggedItem = null;
        private Point itemRelativePosition = new Point();
        private Boolean isDragging = false;

        public MainControl()
        {
            InitializeComponent();
            isDragging = false;
        }

        private void btn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            draggedItem = (Button)sender;
            itemRelativePosition = e.GetPosition(draggedItem);
        }

        private void btn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDragging)
                return;

            isDragging = false;
        }

        private void btn_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging)
                return;

            Point canvasRelativePosition = e.GetPosition(MyCanvas);

            Canvas.SetTop(draggedItem, canvasRelativePosition.Y - itemRelativePosition.Y);
            Canvas.SetLeft(draggedItem, canvasRelativePosition.X - itemRelativePosition.X);
        }
    }
}
