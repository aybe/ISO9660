using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TinyTree
{
    /// <summary>
    /// Returns a path to an image icon for a file or folder.
    /// </summary>
    public class TinyTreeNodeToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fileNode = value as FileNode;
            if (fileNode != null)
            {
                return @"Resources\Images\file.png";
            }

            var directoryNode = value as DirectoryNode;
            if (directoryNode != null)
            {
                return @"Resources\Images\folder.png";
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}