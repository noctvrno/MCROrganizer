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
                var vacantPositionItem = _parent.StandardPositionVacancy[_parent.StandardPositionVacancy.FindIndex(x => x.vacancy == true)];
                TranslateItemHorizontally(this, vacantPositionItem.abscissa);
                vacantPositionItem.vacancy = false;
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

            // Try finding the dragged control in the dictionary and get its position.
            _parent.GamesRelativeAbscissa.TryGetValue(this, out var draggedButtonPosition);

            // Search for a possible collision.
            var collidedControlEntry = _parent.GamesRelativeAbscissa.FirstOrDefault(x => x.Key != this && x.Value.IsInRange(mouseRelativeToItemsControlPosition.X, DBDataContext.Width));
            System.Diagnostics.Debug.WriteLine("Collided with: " + collidedControlEntry);

            if (collidedControlEntry.Key != null && _parent.GamesRelativeAbscissa.TryGetValue(collidedControlEntry.Key, out var collidedButtonPosition))
            {
                TranslateItemHorizontally(collidedControlEntry.Key, draggedButtonPosition);

                // Mark the position as vacant.
                Int32 collidedItemIndex = _parent.StandardPositionVacancy.FindIndex(x => x.abscissa == collidedButtonPosition);
                _parent.StandardPositionVacancy[collidedItemIndex] = (collidedButtonPosition, true);
            }

            System.Diagnostics.Debug.WriteLine(mouseRelativeToItemsControlPosition);
            Canvas.SetLeft(this, mouseRelativeToItemsControlPosition.X - _itemRelativeToParentAbscissa - _mouseRelativeToItemAbscissa);
        }

        // This method translates the control horizontally and updates its abscissa in the main data structure.
        public void TranslateItemHorizontally(DraggableButton itemToTranslate, Double abscissaValue)
        {
            Canvas.SetLeft(itemToTranslate, abscissaValue);
            _itemRelativeToParentAbscissa = abscissaValue;

            // Update dictionary value of the translated item.
            if (_parent.GamesRelativeAbscissa.TryGetValue(itemToTranslate, out _))
                _parent.GamesRelativeAbscissa[itemToTranslate] = abscissaValue;
        }
        #endregion
    }
}
