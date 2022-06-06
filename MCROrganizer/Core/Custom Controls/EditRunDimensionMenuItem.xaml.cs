using MCROrganizer.Core.Extensions;
using MCROrganizer.Core.Utils;
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

namespace MCROrganizer.Core.CustomControls
{
    /// <summary>
    /// Interaction logic for EditRunDimensionMenuItem.xaml
    /// </summary>
    /// 

    public class EditRunDimensionMenuItemDataContext : UserControlDataContext
    {
        #region Members
        private String _dimensionText = String.Empty;
        private String _dimensionValueStr = String.Empty;
        private EditRunDimensionMenuItem _control = null;
        #endregion

        #region Initialization
        public EditRunDimensionMenuItemDataContext(EditRunDimensionMenuItem control)
        {
            _control = control;
        }
        #endregion

        #region Accessors
        public String DimensionText
        {
            get => _dimensionText;
            set
            {
                _dimensionText = value;
                NotifyPropertyChanged(nameof(DimensionText));
            }
        }

        public String DimensionValueStr
        {
            get => _dimensionValueStr;
            set
            {
                if (!Double.TryParse(value, out Double dimensionValue))
                    return;

                DimensionValue = dimensionValue.Clamp(DimensionValueMin, DimensionValueMax);
                _dimensionValueStr = DimensionValue.ToString();
                NotifyPropertyChanged(nameof(DimensionValueStr));
            }
        }

        public Double DimensionValue
        {
            get => (Double)_control.GetValue(EditRunDimensionMenuItem.DimensionValueProperty);
            set => _control.SetValue(EditRunDimensionMenuItem.DimensionValueProperty, value);
        }

        public Double DimensionValueMin
        {
            get => (Double)_control.GetValue(EditRunDimensionMenuItem.DimensionValueMinProperty);
            set => _control.SetValue(EditRunDimensionMenuItem.DimensionValueMinProperty, value);
        }

        public Double DimensionValueMax
        {
            get => (Double)_control.GetValue(EditRunDimensionMenuItem.DimensionValueMaxProperty);
            set => _control.SetValue(EditRunDimensionMenuItem.DimensionValueMaxProperty, value);
        }
        #endregion
    }

    public partial class EditRunDimensionMenuItem : UserControl
    {
        #region Dependency Properties
        public static DependencyProperty DimensionTextProperty = DependencyProperty.Register("DimensionText", typeof(String), typeof(EditRunDimensionMenuItem), new UIPropertyMetadata(String.Empty, OnDimensionTextChanged));
        public static DependencyProperty DimensionValueProperty = DependencyProperty.Register("DimensionValue", typeof(Double), typeof(EditRunDimensionMenuItem), new FrameworkPropertyMetadata(0.0, OnDimensionValueChanged) { BindsTwoWayByDefault = true });
        public static DependencyProperty DimensionValueMinProperty = DependencyProperty.Register("DimensionValueMin", typeof(Double), typeof(EditRunDimensionMenuItem), new UIPropertyMetadata(0.0));
        public static DependencyProperty DimensionValueMaxProperty = DependencyProperty.Register("DimensionValueMax", typeof(Double), typeof(EditRunDimensionMenuItem), new UIPropertyMetadata(Double.PositiveInfinity));

        public static Double GetDimensionValue(DependencyObject dependencyObject)
        {
            return (Double)dependencyObject.GetValue(DimensionValueProperty);
        }

        public static void SetDimensionValue(DependencyObject dependencyObject, Double value)
        {
            dependencyObject.SetValue(DimensionValueProperty, value);
        }

        public static void OnDimensionValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Only update the DimensionValueStr, this will also go through proper checks for DimensionValue and set it accordingly.
            if (dependencyObject is EditRunDimensionMenuItem editRunDimensionMenuItem)
                editRunDimensionMenuItem.CustomMenuItemDataContext.DimensionValueStr = dependencyPropertyChangedEventArgs.NewValue.ToString();
        }

        public static Double GetDimensionValueMin(DependencyObject dependencyObject)
        {
            return (Double)dependencyObject.GetValue(DimensionValueMinProperty);
        }

        public static void SetDimensionValueMin(DependencyObject dependencyObject, Double value)
        {
            dependencyObject.SetValue(DimensionValueMinProperty, value);
        }

        public static void OnDimensionValueMinChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is EditRunDimensionMenuItem editRunDimensionMenuItem)
                editRunDimensionMenuItem.CustomMenuItemDataContext.DimensionValueMin = (Double)dependencyPropertyChangedEventArgs.NewValue;
        }

        public static Double GetDimensionValueMax(DependencyObject dependencyObject)
        {
            return (Double)dependencyObject.GetValue(DimensionValueMaxProperty);
        }

        public static void SetDimensionValueMax(DependencyObject dependencyObject, Double value)
        {
            dependencyObject.SetValue(DimensionValueMaxProperty, value);
        }

        public static void OnDimensionValueMaxChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is EditRunDimensionMenuItem editRunDimensionMenuItem)
                editRunDimensionMenuItem.CustomMenuItemDataContext.DimensionValueMax = (Double)dependencyPropertyChangedEventArgs.NewValue;
        }

        public static String GetDimensionText(DependencyObject dependencyObject)
        {
            return (String)dependencyObject.GetValue(DimensionTextProperty);
        }

        public static void SetDimensionText(DependencyObject dependencyObject, String value)
        {
            dependencyObject.SetValue(DimensionTextProperty, value);
        }

        public static void OnDimensionTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is EditRunDimensionMenuItem editRunDimensionMenuItem)
                editRunDimensionMenuItem.CustomMenuItemDataContext.DimensionText = dependencyPropertyChangedEventArgs.NewValue as String;
        }
        #endregion

        #region Accessors
        public EditRunDimensionMenuItemDataContext CustomMenuItemDataContext { get; set; } = null;
        #endregion

        public EditRunDimensionMenuItem()
        {
            DataContext = CustomMenuItemDataContext = new EditRunDimensionMenuItemDataContext(this);
            InitializeComponent();
        }
    }
}
