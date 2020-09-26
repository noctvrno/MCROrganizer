using MCROrganizer.Core.Commands;
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
using MCROrganizer.Core.ViewModel;

namespace MCROrganizer.Core.CustomControls
{
    /// <summary>
    /// Interaction logic for DraggableButton.xaml
    /// </summary>
    /// 

    // Normally, this would act as a wrapper over the Button's and TextBox' (soon TM) properties in case we would like to use the control statically (instantiate in XAML code)
    // The DraggableButton should only be used dynamically though (so no need to wrap a lot of properties).
    // Whenever the DraggableButton is instantiated, we could always access its DataContext and get/set whatever bound property we would like.
    public class DraggableButtonDataContext
    {
        #region Accessors
        public Double Width { get; set; } = 50.0;
        public Double Height { get; set; } = 50.0;
        #endregion
    }
    
    public partial class DraggableButton : UserControl
    {
        #region Members
        private Control _draggedItem = null;
        private Point _itemRelativePosition = new Point();
        private Boolean _isDragging = false;
        private ControlLogic _parent = null;
        private DraggableButtonDataContext _dataContext = null;
        #endregion

        #region Accessors
        public DraggableButtonDataContext DBDataContext => _dataContext;
        #endregion


        #region Initialization
        public DraggableButton(ControlLogic parent)
        {
            DataContext = _dataContext = new DraggableButtonDataContext();
            InitializeComponent();
            _isDragging = false;
            _parent = parent;
        }
        #endregion

        #region Event handlers
        private void OnButtonPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _draggedItem = (Button)sender;
            _itemRelativePosition = e.GetPosition(_draggedItem);
        }

        private void OnButtonPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging)
                return;

            _isDragging = false;
        }

        private void OnButtonPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging)
                return;

            Point canvasRelativePosition = e.GetPosition(MyCanvas);

            Canvas.SetTop(_draggedItem, canvasRelativePosition.Y - _itemRelativePosition.Y);
            Canvas.SetLeft(_draggedItem, canvasRelativePosition.X - _itemRelativePosition.X);
        }

        public void SetAbscissaValue(Double abscissaValue)
        {
            Canvas.SetLeft(this, abscissaValue);
            
            //if (parent.dictionary.TryGetValue(...))
            //    // update position.
            //else
            //    parent.dictionary.Add(...)
        }
        #endregion
    }
}
