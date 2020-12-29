using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
