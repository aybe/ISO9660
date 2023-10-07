using System.Windows;
using CDROMToolsDemo.Classes;

namespace CDROMToolsDemo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new Model();
            Loaded += OnLoaded;
            Closing += (sender, args) =>
            {
                var model = DataContext as Model;
                model?.Dispose();
            };
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
        }
    }
}