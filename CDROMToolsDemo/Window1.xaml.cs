using System.Windows;
using TinyTree;

namespace CDROMToolsDemo
{
    public partial class Window1
    {
        public Window1()
        {
            InitializeComponent();
            Loaded += Window1_Loaded;
        }

        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            var logbak = new FileNode("log.bak");
            var skycmp = new FileNode("sky.cmp");
            var track01 = new DirectoryNode("track01") {skycmp};
            var common = new DirectoryNode("common") {logbak, track01};
            var wipeout = new DirectoryNode("wipeout") {common};
            var root = new DirectoryNode(@"\") {wipeout};
            UserControl1.Nodes = new[] {root};
        }
    }
}