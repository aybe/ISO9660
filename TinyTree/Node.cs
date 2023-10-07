using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;

namespace TinyTree
{
    /// <summary>
    ///     Represents a node with a name and eventually a parent node.
    /// </summary>
    public abstract class Node : IComparable<Node>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Node _parent;

        /// <summary>
        ///     Create a new instance of <see cref="Node" />.
        /// </summary>
        /// <param name="name">Node name.</param>
        protected Node([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
        }

        /// <summary>
        ///     Gets the full name of this instance.
        /// </summary>
        public string FullName
        {
            get
            {
                var queue = new Stack<string>();
                var node = this;
                while (true)
                {
                    queue.Push(node.Name);
                    var parent = node.Parent;
                    if (parent == null) break;
                    node = parent;
                }
                var fullName = Path.Combine(queue.ToArray());
                return fullName;
            }
        }

        /// <summary>
        ///     Gets the name of this instance.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the parent node of this instance.
        /// </summary>
        public Node Parent => _parent;

        /// <summary>
        ///     Gets or sets an arbitrary object that can store information about this instance.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Node other)
        {
            var d1 = this as DirectoryNode;
            var d2 = other as DirectoryNode;
            var f1 = this as FileNode;
            var f2 = other as FileNode;
            if (d1 != null && d2 != null) return string.CompareOrdinal(d1.Name, d2.Name);
            if (f1 != null && f2 != null) return string.CompareOrdinal(f1.Name, f2.Name);
            if (d1 != null && f2 != null) return -1;
            if (f1 != null && d2 != null) return +1;
            throw new InvalidOperationException();
        }

        private bool Equals(Node other)
        {
            return Equals(FullName, other.FullName);
        }

        /// <summary>
        ///     Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        ///     true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Node) obj);
        }

        /// <summary>
        ///     Serves as the default hash function.
        /// </summary>
        /// <returns>
        ///     A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        /// <summary>
        ///     Sets the parent node of this instance.
        /// </summary>
        /// <param name="node">Node to set as parent.</param>
        public void SetParent([CanBeNull] Node node)
        {
            _parent = node;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}