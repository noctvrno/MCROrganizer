using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MCROrganizer.Core.Utils
{
    public class BooleanToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Double.TryParse((String)parameter, out Double convertedParameter);

            if (value is Boolean bValue && bValue)
                return convertedParameter;

            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Cannot convert from a double back to a boolean for now. There is no need.
        }
    }

    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!value.GetType().IsEnum || !parameter.GetType().IsEnum)
                return DependencyProperty.UnsetValue;

            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Cannot convert a boolean into an enum.
        }
    }

    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Boolean bValue && bValue)
                return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Cannot convert a boolean into an enum.
        }
    }
}
