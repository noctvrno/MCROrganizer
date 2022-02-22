using MCROrganizer.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MCROrganizer.Core.ViewModel;
using MCROrganizer.Core.View;
using MCROrganizer.Core.Extensions;
using MCROrganizer.Core.Utils;
using Newtonsoft.Json;
using System.IO;

namespace MCROrganizer.Core.CustomControls
{
    /// <summary>
    /// Interaction logic for DraggableButton.xaml
    /// </summary>
    /// 
    public enum RunState
    {
        Pending,
        InProgress,
        Finished
    }

    public enum RunParameter
    {
        Width,
        Height,
        Spacing
    }

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
                NotifyPropertyChanged(nameof(Name));
            }
        }

        // Run properties.
        private RunProperties _properties = null;
        public RunProperties Properties
        {
            get => _properties;
            set
            {
                _properties = value;
               _properties?.NotifyPropertyChanged(nameof(_properties.BorderColor));
                NotifyPropertyChanged(nameof(Properties));
            }
        }

        // Width and height (hardcoded for now, might be changed into a two-way databinding in the future).
        private Double _width = 50.0;
        public Double Width
        {
            get => _width;
            set
            {
                _width = value;
                NotifyPropertyChanged(nameof(Width));
            }
        }

        private Double _height = 50.0;
        public Double Height
        {
            get => _height;
            set
            {
                _height = value;
                NotifyPropertyChanged(nameof(Height));
            }
        }

        // Spacing between this run and the next (might be variable in the future).
        public Double Spacing { get; set; } = 20.0;
        #endregion

        #region Two-Way Helper DataBinding Properties
        // Controls whether the button can be interacted with or not.
        private Boolean _isHitTestVisible = false;

        [JsonIgnore]
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

        [JsonIgnore]
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
        private static ImageSource _deleteRunImage = new BitmapImage(new Uri(Path.Combine(PathUtils.ImagePath, "DeleteRun.png")));
        public static ImageSource DeleteRunImage => _deleteRunImage;
        public MCROCommand DeleteRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj => _control.DBParent.RemoveRun(_control)));

        // Rename run command.
        private static ImageSource _renameRunImage = new BitmapImage(new Uri(Path.Combine(PathUtils.ImagePath, "RenameRun.png")));
        public static ImageSource RenameRunImage => _renameRunImage;
        public MCROCommand RenameRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj => MakeButtonEditable()));

        // Set run as current command.
        private static ImageSource _setCurrentRunImage = new BitmapImage(new Uri(Path.Combine(PathUtils.ImagePath, "SetCurrentRun.png")));
        public static ImageSource SetCurrentRunImage => _setCurrentRunImage;
        public MCROCommand SetCurrentRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj => _control.DBParent.SetRunAsCurrent(_control.DBDataContext)));

        // Set run logo command.
        private static ImageSource _setRunLogoImage = new BitmapImage(new Uri(Path.Combine(PathUtils.ImagePath, "SetRunLogo.png")));
        public static ImageSource SetRunLogoImage => _setRunLogoImage;
        public MCROCommand SetRunLogoCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj => SetRunLogo()));

        // Run logo.
        private ImageSource _runLogo = null;
        public ImageSource RunLogo
        {
            get => _runLogo;
            set => _runLogo = value;
        }
        #endregion

        #region Initialization
        public DraggableButtonDataContext(RunProperties properties)
            => Properties = properties;
        #endregion

        #region Communication fields
        private DraggableButton _control = null;

        [JsonIgnore]
        public DraggableButton Control
        {
            get => _control;
            set => _control = value;
        }
        #endregion

        #region Helper methods
        public void MakeButtonEditable(Boolean isEditable = true)
        {
            IsHitTestVisible = IsFocused = isEditable;
        }

        private void SetRunLogo()
        {
            // MUST use a filter here.
            var fileBrowserDialog = new System.Windows.Forms.OpenFileDialog();

            if (fileBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            try
            {
                _runLogo = new BitmapImage(new Uri(fileBrowserDialog.FileName));
            }
            catch
            {
                MessageBox.Show("Could not load the image file. Please try again with a different image.", "MCROrganizer", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Update the main view after uploading the run logo.
            _control.DBParent.NotifyPropertyChanged("RunInProgress");
            _control.DBParent.NotifyPropertyChanged("IsCurrentRunLogoSet");
        }
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
        public ControlLogic DBParent => _parent;
        #endregion

        #region Initialization
        public DraggableButton(ControlLogic parent, DraggableButtonDataContext data = null, RunProperties properties = null)
        {
            // data will be null for every instantiation of a run, except for a load because in that case, we deserialize the data from a json file.
            DataContext = _dataContext = data ?? new DraggableButtonDataContext(properties);
            _dataContext.Control = this;
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

    public class RunProperties : UserControlDataContext
    {
        private SolidColorBrush _borderColor = null;
        public SolidColorBrush BorderColor
        {
            get => _borderColor;
            set => _borderColor = value;
        }

        private SolidColorBrush _backgroundColor = null;
        public SolidColorBrush BackgroundColor
        {
            get => _backgroundColor;
            set => _backgroundColor = value;
        }

        private SolidColorBrush _fontColor = null;
        public SolidColorBrush FontColor
        {
            get => _fontColor;
            set => _fontColor = value;
        }

        private RunState _state = RunState.Pending;
        public RunState State => _state;

        public RunProperties(RunState state)
        {
            _state = state;
            _borderColor = _state switch
            {
                RunState.Pending => new SolidColorBrush(Colors.Red),
                RunState.InProgress => new SolidColorBrush(Colors.Yellow),
                RunState.Finished => new SolidColorBrush(Colors.Green),
                RunState _ => throw new NotSupportedException("Unreachable")
            };

            _backgroundColor = new SolidColorBrush(Colors.Transparent);
            _fontColor = new SolidColorBrush(Colors.Black);
        }
    }
}
