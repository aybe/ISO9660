using JetBrains.Annotations;

namespace TinyTree
{
    /// <summary>
    ///     Represents a file node.
    /// </summary>
    public sealed class FileNode : Node
    {
        /// <summary>
        ///     Create a new instance of <see cref="FileNode" />.
        /// </summary>
        /// <param name="name">File name.</param>
        public FileNode([NotNull] string name) : base(name)
        {
        }
    }
}