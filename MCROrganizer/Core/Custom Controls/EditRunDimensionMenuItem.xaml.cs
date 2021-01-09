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
                NotifyPropertyChanged("DimensionText");
            }
        }

        public String DimensionValueStr
        {
            get => _dimensionValueStr;
            set
            {
                if (Double.TryParse(value, out Double dimensionValue))
                {
                    if (dimensionValue.IsBetween(DimensionValueMin, DimensionValueMax))
                        DimensionValue = dimensionValue;
                    else
                        DimensionValue = dimensionValue < DimensionValueMin ? DimensionValueMin : DimensionValueMax;

                    _dimensionValueStr = DimensionValue.ToString();
                }
                else
                    return;

                NotifyPropertyChanged("DimensionValueStr");
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
        public static DependencyProperty DimensionValueProperty = DependencyProperty.RegisterAttached("DimensionValue", typeof(Double), typeof(EditRunDimensionMenuItem), new UIPropertyMetadata(0.0));
        public static DependencyProperty DimensionValueMinProperty = DependencyProperty.RegisterAttached("DimensionValueMin", typeof(Double), typeof(EditRunDimensionMenuItem), new UIPropertyMetadata(0.0));
        public static DependencyProperty DimensionValueMaxProperty = DependencyProperty.RegisterAttached("DimensionValueMax", typeof(Double), typeof(EditRunDimensionMenuItem), new UIPropertyMetadata(Double.PositiveInfinity));

        public static Double GetDimensionValue(DependencyObject dependencyObject)
        {
            return (Double)dependencyObject.GetValue(DimensionValueProperty);
        }

        public static void SetDimensionValue(DependencyObject dependencyObject, Double value)
        {
            dependencyObject.SetValue(DimensionValueProperty, value);
        }

        public static Double GetDimensionValueMin(DependencyObject dependencyObject)
        {
            return (Double)dependencyObject.GetValue(DimensionValueMinProperty);
        }

        public static void SetDimensionValueMin(DependencyObject dependencyObject, Double value)
        {
            dependencyObject.SetValue(DimensionValueMinProperty, value);
        }

        public static Double GetDimensionValueMax(DependencyObject dependencyObject)
        {
            return (Double)dependencyObject.GetValue(DimensionValueMaxProperty);
        }

        public static void SetDimensionValueMax(DependencyObject dependencyObject, Double value)
        {
            dependencyObject.SetValue(DimensionValueMaxProperty, value);
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
