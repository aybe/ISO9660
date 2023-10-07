using System;
using System.Collections.Generic;
using System.Linq;
using CDROMTools;
using JetBrains.Annotations;
using TinyTree;

namespace CDROMToolsDemo.Classes
{
    public static class IsoTreeHelper
    {
        /// <summary>
        ///     Create a hierarchy from a list of files.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static IEnumerable<DirectoryNode> ToNodes([NotNull] CDROMFile[] files)
        {
            if (files == null) throw new ArgumentNullException(nameof(files));
            var root = new DirectoryNode(@"\");
            foreach (var file in files)
            {
                var queue = new Queue<string>();
                var split = file.DirectoryName.Split(new[] {@"\"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in split) queue.Enqueue(s);
                queue.Enqueue(file.FileName);
                var current = root;
                while (queue.Count > 0)
                {
                    var dequeue = queue.Dequeue();
                    if (queue.Count == 0) // last is file
                    {
                        var node = new FileNode(dequeue) {Tag = file};
                        current.Add(node);
                        break;
                    }

                    // create/update directory
                    var item = current.Directories.SingleOrDefault(s => s.Name == dequeue);
                    if (item == null)
                    {
                        item = new DirectoryNode(dequeue);
                        current.Add(item);
                    }
                    current = item;
                }
            }

            return new[] {root};
        }
    }
}