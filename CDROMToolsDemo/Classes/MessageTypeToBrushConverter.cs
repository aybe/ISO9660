using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CDROMToolsDemo.Classes
{
    public class MessageTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            Color color;
            var type = (MessageType) value;
            switch (type)
            {
                case MessageType.Normal:
                    color = Colors.Green;
                    break;
                case MessageType.Warning:
                    color = Colors.DarkOrange;
                    break;
                case MessageType.Error:
                    color = Colors.Red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}