namespace RemoteOS.OpenComputers.Components
{
    [Obsolete("Store all data on the server, not on the remote machine.")]
    [Component("drive")]
    public class DriveComponent : Component {
        int? _sectorSize;
        string? _label;
        public DriveComponent(Machine parent, Guid address) : base(parent, address)
        {
        }
        public async Task<byte> ReadByte(int offset) => (byte)(await Invoke("readByte", offset))[0];
        public async Task WriteByte(int offset, byte data) => await Invoke("writeByte", offset, data);
        public async Task<string> ReadSector(int offset) => (await Invoke("readSector", offset))[0];
        public async Task WriteSector(int offset, string data) => await Invoke("readSector", offset, $@"""{data}""");
        public async Task<int> GetPlatterCount() => (await Invoke("getPlatterCount"))[0];
        public async Task<int> GetCapacity() => (await Invoke("getCapacity"))[0];
        public async Task<int> GetSectorSize() => _sectorSize ??= (await Invoke("getSectorSize"))[0];
        public async Task<string> GetLabel() => _label ??= (await Invoke("getLabel"))[0];
        public async Task SetLabel(string label) => _label = (await Invoke("setLabel", $@"""{label}"""))[0];
#if ROS_PROPERTIES
#if ROS_PROPS_UNCACHED
        public int PlatterCount => GetPlatterCount().Result;
        public int Capacity => GetCapacity().Result;
#endif
        public int SectorSize => GetSectorSize().Result;
        public string Label
        {
            get => GetLabel().Result;
            set => SetLabel(value);
        }
#endif
    }
}
