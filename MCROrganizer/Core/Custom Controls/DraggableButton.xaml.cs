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
using MCROrganizer.Core.Utils;

namespace MCROrganizer.Core.CustomControls
{
    /// <summary>
    /// Interaction logic for DraggableButton.xaml
    /// </summary>
    /// 

    // Normally, this would act as a wrapper over the Button's and TextBox' (soon TM) properties in case we would like to use the control statically (instantiate in XAML code)
    // The DraggableButton should only be used dynamically though (so no need to wrap a lot of properties).
    // Whenever the DraggableButton is instantiated, we could always access its DataContext and get/set whatever bound property we would like.
    public class DraggableButtonDataContext : UserControlDataContext
    {
        #region Customization Properties
        // Run name.
        private String _name = "DS";
        public String Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        // State of the run.
        private RunState _runState = RunState.Pending;
        public RunState RunState
        {
            get => _runState;
            set
            {
                _runState = value;
                NotifyPropertyChanged("RunState");
                NotifyPropertyChanged("RunStateColor");
            }
        }
        public SolidColorBrush RunStateColor
        {
            get
            {
                return _runState switch
                {
                    RunState.Pending => new SolidColorBrush(Colors.Red),
                    RunState.InProgress => new SolidColorBrush(Colors.Yellow),
                    RunState.Finished => new SolidColorBrush(Colors.Green),
                    RunState _ => throw new NotSupportedException("Unreachable")
                };
            }
        }

        // Width and height (hardcoded for now, might be changed into a two-way databinding in the future).
        public Double Width { get; set; } = 50.0;
        public Double Height { get; set; } = 50.0;
        #endregion

        #region Two-Way Helper DataBinding Properties
        // Controls whether the button can be interacted with or not.
        private Boolean _isHitTestVisible = false;
        public Boolean IsHitTestVisible
        {
            get => _isHitTestVisible;
            set
            {
                _isHitTestVisible = value;
                NotifyPropertyChanged("IsHitTestVisible");
            }
        }

        // Changes the focus to the button and highlights the text within it.
        private Boolean _isFocused = false;
        public Boolean IsFocused
        {
            get => _isFocused;
            set
            {
                _isFocused = value;
                NotifyPropertyChanged("IsFocused");
            }
        }

        // Delete run command.
        private static ImageSource _deleteRunImage = new BitmapImage(new Uri(PathUtils.ImagePath + "DeleteRun.png"));
        public ImageSource DeleteRunImage => _deleteRunImage;
        public ICommand DeleteRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj => _control.DBParent.RemoveRun(_control)));

        // Rename run command.
        private static ImageSource _renameRunImage = new BitmapImage(new Uri(PathUtils.ImagePath + "RenameRun.png"));
        public ImageSource RenameRunImage => _renameRunImage;
        public ICommand RenameRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj => MakeButtonEditable()));

        // Set run as current command.
        private static ImageSource _setCurrentRunImage = new BitmapImage(new Uri(PathUtils.ImagePath + "SetCurrentRun.png"));
        public ImageSource SetCurrentRunImage => _setCurrentRunImage;
        public ICommand SetCurrentRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj => _control.DBParent.SetRunAsCurrent(_control)));
        #endregion

        #region Communication fields
        private DraggableButton _control = null;
        #endregion

        #region Helper methods
        public void MakeButtonEditable(Boolean isEditable = true)
        {
            IsHitTestVisible = IsFocused = isEditable;
        }
        #endregion

        public DraggableButtonDataContext(DraggableButton control) => _control = control;
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
        public ControlLogic DBParent => _parent;
        #endregion

        #region Initialization
        public DraggableButton(ControlLogic parent)
        {
            DataContext = _dataContext = new DraggableButtonDataContext(this);
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

            if (_parent.AbscissaByRun.TryGetValue(this, out Double draggedItemAbscissaValue))
                _parent.TranslateItemHorizontally(this, draggedItemAbscissaValue);

            _isDragging = false;
        }

        private void OnButtonPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging)
                return;

            // Get the position of the mouse relative to the control.
            Point mouseRelativeToItemsControlPosition = e.GetPosition((_parent.MainControl as MainControl).buttonsItemsControl);

            // Search for a possible collision and swap the items.
            var collidedControlEntry = _parent.AbscissaByRun.FirstOrDefault(x => x.Key != this && mouseRelativeToItemsControlPosition.X.IsInRange(x.Value + DBDataContext.Width / 2, DBDataContext.Width / 2));
            if (collidedControlEntry.Key != null && _parent.AbscissaByRun.TryGetValue(collidedControlEntry.Key, out var collidedButtonPosition))
                _parent.SwapDraggedItemOnCollision(this, collidedControlEntry.Key);

            _parent.TranslateItemHorizontally(this, mouseRelativeToItemsControlPosition.X - _mouseRelativeToItemAbscissa);
        }

        private void OnLabelPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            DBDataContext.MakeButtonEditable();
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            DBDataContext.MakeButtonEditable(false);
        }
        #endregion
    }
}
