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
using MCROrganizer.Core.View;
using MCROrganizer.Core.Extensions;

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
        private Double _mouseRelativeToItemAbscissa = 0.0;
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
            _mouseRelativeToItemAbscissa = e.GetPosition(this).X;
        }

        private void OnButtonPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging)
                return;

            if (_parent.GamesRelativeAbscissa.TryGetValue(this, out Double draggedItemAbscissaValue))
                TranslateItemHorizontally(this, draggedItemAbscissaValue);

            _isDragging = false;
        }

        private void OnButtonPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging)
                return;

            // Get the position of the mouse relative to the control.
            Point mouseRelativeToItemsControlPosition = e.GetPosition((_parent.MainControl as MainControl).buttonsItemsControl);

            // Search for a possible collision and swap the items.
            var collidedControlEntry = _parent.GamesRelativeAbscissa.FirstOrDefault(x => x.Key != this && mouseRelativeToItemsControlPosition.X.IsInRange(x.Value + DBDataContext.Width / 2, DBDataContext.Width / 2));
            if (collidedControlEntry.Key != null && _parent.GamesRelativeAbscissa.TryGetValue(collidedControlEntry.Key, out var collidedButtonPosition))
                SwapDraggedItemOnCollision(collidedControlEntry.Key);

            Canvas.SetLeft(this, mouseRelativeToItemsControlPosition.X - _mouseRelativeToItemAbscissa);
        }

        // This method translates the control horizontally.
        public void TranslateItemHorizontally(DraggableButton itemToTranslate, Double abscissaValue)
        {
            Canvas.SetLeft(itemToTranslate, abscissaValue);
        }

        // This method moves the collidedItem to the draggedItem position and updates the dictionary according to the swap.
        private void SwapDraggedItemOnCollision(DraggableButton collidedItem)
        {
            // Update the dictionary so that the draggedItem has the collidedItem's position.
            if (_parent.GamesRelativeAbscissa.TryGetValue(this, out Double draggedItemAbscissaValue) && _parent.GamesRelativeAbscissa.TryGetValue(collidedItem, out Double collidedItemAbscissaValue))
            {
                // Physically move the collideditem to the draggedItem standard position.
                TranslateItemHorizontally(collidedItem, draggedItemAbscissaValue);

                // Swap the values of the draggedItem and the collidedItem.
                _parent.GamesRelativeAbscissa[this] = collidedItemAbscissaValue;
                _parent.GamesRelativeAbscissa[collidedItem] = draggedItemAbscissaValue;
            }
        }
        #endregion
    }
}
