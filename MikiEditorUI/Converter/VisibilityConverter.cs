namespace MikiEditorUI.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (bool)value;

            if (visibility)
            {
                return "Visible";
            }

            return "Hidden";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (bool)value;

            if (visibility)
            {
                return "Visible";
            }

            return "Hidden";
        }
    }
}
