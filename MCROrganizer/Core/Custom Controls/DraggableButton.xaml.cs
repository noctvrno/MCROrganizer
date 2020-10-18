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
        private Double _itemRelativeToParentAbscissa = 0.0; // Parent means the ItemsControl defined in the main view.
        private Boolean _isDragging = false;
        private ControlLogic _parent = null;
        private DraggableButtonDataContext _dataContext = null;
        #endregion

        #region Accessors
        public DraggableButtonDataContext DBDataContext => _dataContext;
        public Double ItemRelativeToParentAbscissa
        {
            get => _itemRelativeToParentAbscissa;
            set => _itemRelativeToParentAbscissa = value;
        }
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

            // Find a vacant position and move it there.
            try
            {
                Int32 vacantPositionIndex = _parent.StandardPositionVacancy.FindIndex(x => x.vacancy == true);
                Double vacantPositionAbscissa = _parent.StandardPositionVacancy[vacantPositionIndex].abscissa;
                TranslateItemHorizontally(this, vacantPositionAbscissa);
                _parent.StandardPositionVacancy[vacantPositionIndex] = (vacantPositionAbscissa, false);
            }
            catch { }

            _isDragging = false;
        }

        private void OnButtonPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging)
                return;

            // Get the position of the mouse relative to the control.
            Point mouseRelativeToItemsControlPosition = e.GetPosition((_parent.MainControl as MainControl).buttonsItemsControl);

            // Try finding the dragged control in the dictionary, get its position and mark it as vacant.
            Double draggedButtonPosition = 0.0;
            Int32 draggedItemIndex = -1;
            if (_parent.GamesRelativeAbscissa.TryGetValue(this, out draggedButtonPosition))
            {
                draggedItemIndex = _parent.StandardPositionVacancy.FindIndex(x => x.abscissa == draggedButtonPosition);
                _parent.StandardPositionVacancy[draggedItemIndex] = (draggedButtonPosition, true);
            }

            // Search for a possible collision.
            var collidedControlEntry = _parent.GamesRelativeAbscissa.FirstOrDefault(x => x.Key != this && mouseRelativeToItemsControlPosition.X.IsInRange(x.Value + DBDataContext.Width / 2, DBDataContext.Width / 2));
            if (collidedControlEntry.Key != null && _parent.GamesRelativeAbscissa.TryGetValue(collidedControlEntry.Key, out var collidedButtonPosition))
            {
                SwapDraggedItemOnCollision(collidedControlEntry.Key);

                // Find the index of the standard position of the button we just collided with.
                Int32 collidedItemIndex = _parent.StandardPositionVacancy.FindIndex(x => x.abscissa == collidedButtonPosition);

                // Mark the found position as vacant in the standard position list (vacancy update).
                _parent.StandardPositionVacancy[collidedItemIndex] = (collidedButtonPosition, true);

                // Mark the entry position (of the current dragged button) as non-vacant.
                _parent.StandardPositionVacancy[draggedItemIndex] = (draggedButtonPosition, false);
            }

            Canvas.SetLeft(this, mouseRelativeToItemsControlPosition.X - _mouseRelativeToItemAbscissa);
        }

        // This method translates the control horizontally and updates its abscissa in the main data structure.
        public void TranslateItemHorizontally(DraggableButton itemToTranslate, Double abscissaValue)
        {
            Canvas.SetLeft(itemToTranslate, abscissaValue);
            _itemRelativeToParentAbscissa = abscissaValue;
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
