using EasyJSON;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Obsolete("Store all data on the server, not on the remote machine.")]
    public partial class FilesystemComponent : Component
    {
        string? _label;
        int? _totalSpace;

        public FilesystemComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Creates a directory at the specified absolute path in the file system. Creates parent directories, if necessary.
        /// </summary>
        /// <param name="path">Absolute path</param>
        /// <returns>true if the directory creation was successful</returns>
        public partial Task<bool> MakeDirectory(string path);

        /// <param name="path">Absolute path</param>
        /// <returns>Whether an object exists at the specified absolute path in the file system.</returns>
        public partial Task<bool> Exists(string path);

        /// <param name="path">Absolute path</param>
        /// <returns>whether the object at the specified absolute path in the file system is a directory.</returns>
        public partial Task<bool> IsDirectory(string path);

        /// <summary>
        /// Renames/moves an object from the first specified absolute path in the file system to the second.
        /// </summary>
        /// <param name="from">Absolute path of original file</param>
        /// <param name="to">Absolute path of destination file</param>
        /// <returns>true if file was renamed successfully</returns>
        public partial Task<bool> Rename(string from, string to);

        /// <param name="path">Absolute path</param>
        /// <returns>A list of names of objects in the directory at the specified absolute path in the file system.</returns>
        public async Task<IEnumerable<string>> List(string path) => ((await GetInvoker()(path))[0]).Linq.Select(x => x.Value.Value);

        /// <param name="path">Absolute path</param>
        /// <returns>The (real world) timestamp of when the object at the specified absolute path in the file system was modified.</returns>
        public async Task<DateTime> LastModified(string path) => new DateTime().AddSeconds((await GetInvoker()(path))[0]);

        /// <summary>
        /// Removes the object at the specified absolute path in the file system.
        /// </summary>
        /// <param name="path">Absolute path</param>
        /// <returns>true if file was removed</returns>
        public partial Task<bool> Remove(string path);

        /// <param name="path">Absolute path</param>
        /// <returns>The size of the object at the specified absolute path in the file system.</returns>
        public partial Task<int> Size(string path);

        /// <summary>
        /// Opens a new file descriptor and returns its handle.
        /// </summary>
        /// <param name="path">Absolute path</param>
        /// <param name="mode">Stream mode</param>
        /// <returns>File descriptor handle</returns>
        /// <exception cref="ArgumentException">This stream mode is not supported</exception>
        public async Task<FilesystemStream> Open(string path, string mode = "r")
        {
            if (!new string[] { "r", "rb", "w", "wb", "a", "ab" }.Contains(mode)) throw new ArgumentException("Unsupported mode");
            return new FilesystemStream(this, (await GetInvoker()(path, mode))[0]);
        }

        /// <returns>Whether the file system is read-only.</returns>
        public partial Task<bool> IsReadOny();

        /// <returns>The currently used capacity of the file system, in bytes.</returns>
        public async Task<int> GetUsedSpace() => (await Invoke("spaceUsed"))[0];

        /// <returns>The overall capacity of the file system, in bytes.</returns>
        public async Task<int> GetTotalSpace() => _totalSpace ??= (await Invoke("spaceTotal"))[0];

        /// <returns>The current label of the drive.</returns>
        public async Task<string> GetLabel() => _label ??= (await GetInvoker()())[0];

        /// <summary>
        /// Sets the label of the drive.
        /// </summary>
        /// <param name="label">The new label</param>
        /// <returns>The new value, which may be truncated.</returns>
        public async Task SetLabel(string label) => _label = (await GetInvoker()(label))[0];

#if ROS_PROPERTIES
#if ROS_PROPS_UNCACHED
        public bool ReadOnly => IsReadOny().Result;

        public int UsedSpace => GetUsedSpace().Result;
#endif
        public int TotalSpace => GetTotalSpace().Result;

        public string Label
        {
            get => GetLabel().Result;
            set => SetLabel(value);
        }
#endif

        public class FilesystemStream : IDisposable
        {
            public FilesystemComponent Parent { get; }
            int Handle { get; }

            public FilesystemStream(FilesystemComponent component, int handle)
            {
                Parent = component;
                Handle = handle;
            }

            /// <summary>
            /// Reads up to the specified amount of data from an open file descriptor
            /// </summary>
            /// <param name="count">How much bytes to read</param>
            /// <returns>Data, empty string when EOF is reached</returns>
            public async Task<string> Read(int count) => (await Parent.GetInvoker()(Handle, count))[0];

            /// <summary>
            /// Writes the specified data to an open file descriptor
            /// </summary>
            /// <param name="data">Data to write</param>
            /// <returns>true if data was written successfully</returns>
            public async Task<bool> Write(string data) => (await Parent.GetInvoker()(Handle, data))[0];

            /// <summary>
            /// Seeks in an open file descriptor
            /// </summary>
            /// <param name="whence">Seek mode</param>
            /// <param name="offset">Offset</param>
            /// <returns>The new pointer position.</returns>
            public async Task<int> Seek(string whence, int offset) => (await Parent.GetInvoker()(Handle, whence, offset))[0];

            /// <summary>
            /// Closes an open file descriptor
            /// </summary>
            public Task Close() => Parent.GetInvoker()(Handle);

            public void Dispose()
            {
                Close();
            }
        }
    }
}
