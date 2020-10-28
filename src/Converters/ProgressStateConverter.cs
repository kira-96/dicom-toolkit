using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Shell;

namespace SimpleDICOMToolkit.Converters
{
    public class ProgressStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool indeterminate && indeterminate)
                return TaskbarItemProgressState.Indeterminate;

            return TaskbarItemProgressState.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskbarItemProgressState state &&
                state == TaskbarItemProgressState.Indeterminate)
                return true;

            return false;
        }
    }
}
