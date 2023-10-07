using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TinyTree
{
    /// <summary>
    /// An Explorer-like control with folder and file panes.
    /// </summary>
    public partial class TinyTreeControl
    {
        public static readonly DependencyProperty NodesProperty = DependencyProperty.Register(
            "Nodes", typeof (IEnumerable<DirectoryNode>), typeof (TinyTreeControl),
            new PropertyMetadata(default(IEnumerable<DirectoryNode>), OnNodesPropertyChanged));

        public static readonly DependencyProperty SelectedNodeProperty = DependencyProperty.Register(
            "SelectedNode", typeof (DirectoryNode), typeof (TinyTreeControl),
            new PropertyMetadata(default(DirectoryNode)));

        public static readonly DependencyProperty FileViewProperty = DependencyProperty.Register(
            "FileView", typeof (ViewBase), typeof (TinyTreeControl), new PropertyMetadata(default(ViewBase)));


        public static readonly DependencyProperty FolderTemplateProperty = DependencyProperty.Register(
            "FolderTemplate", typeof (DataTemplate), typeof (TinyTreeControl),
            new PropertyMetadata(default(DataTemplate)));


        public TinyTreeControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the collection of nodes to display.
        /// </summary>
        public IEnumerable<DirectoryNode> Nodes
        {
            get { return (IEnumerable<DirectoryNode>) GetValue(NodesProperty); }
            set { SetValue(NodesProperty, value); }
        }

        /// <summary>
        ///     Gets or sets currently selected node (read-only).
        /// </summary>
        public DirectoryNode SelectedNode
        {
            get { return (DirectoryNode) GetValue(SelectedNodeProperty); }
            private set { SetValue(SelectedNodeProperty, value); }
        }

        /// <summary>
        ///     Gets or sets view for file pane.
        /// </summary>
        public ViewBase FileView
        {
            get { return (ViewBase) GetValue(FileViewProperty); }
            set { SetValue(FileViewProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the template for folder pane.
        /// </summary>
        public DataTemplate FolderTemplate
        {
            get { return (DataTemplate) GetValue(FolderTemplateProperty); }
            set { SetValue(FolderTemplateProperty, value); }
        }

        private static void OnNodesPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as TinyTreeControl;
            if (control != null) control.SelectedNode = null;
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var node = e.NewValue as DirectoryNode;
            SelectedNode = node;
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // select folder in left pane no matter what situation ...

            var item = sender as ListViewItem;
            var node = item?.Content as DirectoryNode;
            if (node == null) return;

            var nodes = (IEnumerable<DirectoryNode>) TreeView.ItemsSource;
            if (nodes == null) return;

            var queue = new Stack<Node>();
            queue.Push(node);
            var parent = node.Parent;
            while (parent != null)
            {
                queue.Push(parent);
                parent = parent.Parent;
            }

            var generator = TreeView.ItemContainerGenerator;
            while (queue.Count > 0)
            {
                var dequeue = queue.Pop();
                TreeView.UpdateLayout();
                var treeViewItem = (TreeViewItem) generator.ContainerFromItem(dequeue);
                if (queue.Count > 0) treeViewItem.IsExpanded = true;
                else treeViewItem.IsSelected = true;
                generator = treeViewItem.ItemContainerGenerator;
            }
        }
    }
}