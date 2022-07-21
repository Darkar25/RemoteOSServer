namespace RemoteOS.OpenComputers.Components
{
    [Obsolete("Store all data on the server, not on the remote machine.")]
    public class FilesystemComponent : Component
    {
        string? _label;
        int? _totalSpace;

        public FilesystemComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        public async Task<bool> MakeDirectory(string path) => (await Invoke("makeDirectory", $@"""{path}"""))[0];
        public async Task<bool> Exists(string path) => (await Invoke("exists", $@"""{path}"""))[0];
        public async Task<bool> IsDirectory(string path) => (await Invoke("isDirectory", $@"""{path}"""))[0];
        public async Task<bool> Rename(string from, string to) => (await Invoke("rename", $@"""{from}""", $@"""{to}"""))[0];
        public async Task<IEnumerable<string>> List(string path) => (await Invoke("list", $@"""{path}"""))[0].Linq.Select(x => x.Value.Value);
        public async Task<int> LastModified(string path) => (await Invoke("lastModified", $@"""{path}"""))[0];
        public async Task<bool> Remove(string path) => (await Invoke("remove", $@"""{path}"""))[0];
        public async Task<int> Size(string path) => (await Invoke("size", $@"""{path}"""))[0];
        public async Task<FilesystemStream> Open(string path, string mode = "r")
        {
            if (!new string[] { "r", "rb", "w", "wb", "a", "ab" }.Contains(mode)) throw new ArgumentException("Unsupported mode");
            return new FilesystemStream(this, (await Invoke("open", $@"""{path}""", mode))[0]);
        }
        public async Task<bool> IsReadOny() => (await Invoke("isReadOnly"))[0];
        public async Task<int> GetUsedSpace() => (await Invoke("spaceUsed"))[0];
        public async Task<int> GetTotalSpace() => _totalSpace ??= (await Invoke("spaceTotal"))[0];
        public async Task<string> GetLabel() => _label ??= (await Invoke("getLabel"))[0];
        public async Task SetLabel(string label) => _label = (await Invoke("setLabel", $@"""{label}"""))[0];

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

            public async Task<string> Read(int count) => (await Parent.Invoke("read", Handle, count))[0];
            
            public async Task<bool> Write(string data) => (await Parent.Invoke("write", Handle, $@"""{data}"""))[0];

            public async Task<int> Seek(string whence, int offset) => (await Parent.Invoke("seek", Handle, $@"""{whence}""", offset))[0];

            public async Task Close() => await Parent.Invoke("close", Handle);

            public void Dispose()
            {
                Close();
            }
        }
    }
}
