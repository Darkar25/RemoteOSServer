namespace RemoteOS.OpenComputers.Components
{
    [Obsolete("Store all data on the server, not on the remote machine.")]
    [Component("eeprom")]
    public class EEPROMComponent : Component
    {
        string? _bytes;
        string? _label;
        int? _size;
        int? _dataSize;
        string? _data;
        public EEPROMComponent(Machine parent, Guid address) : base(parent, address)
        {
        }
        public async Task<bool> MakeReadonly(string checksum) => (await Invoke("makeReadonly", checksum))[0];

        public async Task<string> Get() => _bytes ??= (await Invoke("get"))[0];
        public async Task Set(string bytes)
        {
            _bytes = bytes;
            await Invoke("set", $@"""{bytes}""");
        }
        public async Task<string> GetLabel() => _label ??= (await Invoke("getLabel"))[0];
        public async Task SetLabel(string label)
        {
            _label = label;
            await Invoke("setLabel", $@"""{label}""");
        }
        public async Task<int> GetSize() => _size ??= (await Invoke("getSize"))[0];
        public async Task<int> GetDataSize() => _dataSize ??= (await Invoke("getDataSize"))[0];
        public async Task<string> GetData() => _data ??= (await Invoke("getData"))[0];
        public async Task SetData(string data)
        {
            _data = data;
            await Invoke("setData", $@"""{data}""");
        }
        public async Task<string> GetChecksum() => (await Invoke("getChecksum"))[0];

#if ROS_PROPERTIES
        public string Bytes
        {
            get => Get().Result;
            set => Set(value);
        }
        public string Label
        {
            get => GetLabel().Result;
            set => SetLabel(value);
        }
        public int Size => GetSize().Result;
        public int DataSize => GetDataSize().Result;
        public string Data
        {
            get => GetData().Result;
            set => SetData(value);
        }
#if ROS_PROPS_UNCACHED
        public string Checksum => GetChecksum().Result;
#endif
#endif
    }
}
