using MCROrganizer.Core.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using MCROrganizer.Core.View;
using MCROrganizer.Core.ViewModel;
using MCROrganizer.Core.Commands;

namespace MCROrganizer.Core.CustomControls
{
    /// <summary>
    /// Interaction logic for EditRunDimensionMenuItem.xaml
    /// </summary>
    /// 

    public enum CustomizableRunElements
    {
        Border,
        Background,
        Font
    };

    public class DesignRunMenuItemDataContext : UserControlDataContext
    {
        #region Members
        private DesignRunMenuItem _control = null;
        #endregion

        #region Accessors
        private static ImageSource _designPendingRunImage = new BitmapImage(new Uri(Path.Combine(PathUtils.ImagePath, "ColorPalettePendingRun.png")));
        private static ImageSource _designInProgressRunImage = new BitmapImage(new Uri(Path.Combine(PathUtils.ImagePath, "ColorPaletteInProgressRun.png")));
        private static ImageSource _designFinishedRunImage = new BitmapImage(new Uri(Path.Combine(PathUtils.ImagePath, "ColorPaletteFinishedRun.png")));
        private static ImageSource _designBorderColorImage = new BitmapImage(new Uri(Path.Combine(PathUtils.ImagePath, "DesignBorders.png")));
        private static ImageSource _designBackgroundColorImage = new BitmapImage(new Uri(Path.Combine(PathUtils.ImagePath, "DesignBackground.png")));
        private static ImageSource _designFontColorImage = new BitmapImage(new Uri(Path.Combine(PathUtils.ImagePath, "DesignFontColor.png")));

        public ImageSource DesignPendingRunImage => _designPendingRunImage;
        public ImageSource DesignInProgressRunImage => _designInProgressRunImage;
        public ImageSource DesignFinishedRunImage => _designFinishedRunImage;
        public ImageSource DesignBorderColorImage => _designBorderColorImage;
        public ImageSource DesignBackgroundColorImage => _designBackgroundColorImage;
        public ImageSource DesignFontColorImage => _designFontColorImage;

        public ControlLogic ParentControlLogic { get; set; } = null;

        public ICommand DesignRunCommand => new MCROCommand(obj => ParentControlLogic.DesignRun(RunState, (CustomizableRunElements)obj));

        public RunState RunState
        {
            get => (RunState)_control.GetValue(DesignRunMenuItem.RunStateProperty);
            set => _control.SetValue(DesignRunMenuItem.RunStateProperty, value);
        }
        #endregion

        #region Initialization
        public DesignRunMenuItemDataContext(DesignRunMenuItem control)
        {
            _control = control;
        }
        #endregion
    }

    public partial class DesignRunMenuItem : MenuItem
    {
        #region Dependency Properties
        public static DependencyProperty RunStateProperty = DependencyProperty.Register("RunState", typeof(RunState), typeof(DesignRunMenuItem), new FrameworkPropertyMetadata(RunState.Pending, OnRunStateChanged));

        public static RunState GetRunState(DependencyObject dependencyObject)
        {
            return (RunState)dependencyObject.GetValue(RunStateProperty);
        }

        public static void SetRunState(DependencyObject dependencyObject, RunState value)
        {
            dependencyObject.SetValue(RunStateProperty, value);
        }

        public static void OnRunStateChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is DesignRunMenuItem designRunMenuItem)
                designRunMenuItem.DesignRunMenuItemDataContext.RunState = (RunState)dependencyPropertyChangedEventArgs.NewValue;
        }
        #endregion

        public DesignRunMenuItemDataContext DesignRunMenuItemDataContext { get; set; }

        public DesignRunMenuItem()
        {
            DataContext = DesignRunMenuItemDataContext = new DesignRunMenuItemDataContext(this);
            InitializeComponent();
        }

        /// <summary>
        /// This will extract the parent ControlLogic by going up the Visual Tree. A bit tricky since the next logical parent is a ContextMenu.
        /// </summary>
        private void OnDesignRunMenuItemLoaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(this);
            while (parentObject is not System.Windows.Controls.ContextMenu)
            {
                parentObject = VisualTreeHelper.GetParent(parentObject);
            }

            DesignRunMenuItemDataContext.ParentControlLogic = ((parentObject as ContextMenu)?.PlacementTarget as MainControl)?.DataContext as ControlLogic;
        }
    }
}
