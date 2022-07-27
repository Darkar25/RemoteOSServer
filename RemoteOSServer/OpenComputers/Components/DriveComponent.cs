namespace RemoteOS.OpenComputers.Components
{
    [Obsolete("Store all data on the server, not on the remote machine.")]
    [Component("drive")]
    public class DriveComponent : Component {
        int? _capacity;
        int? _platterCount;
        string? _label;
        public DriveComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// The size of a single sector on the drive, in bytes.
        /// </summary>
        public int SectorSize => 512; //This is hardcoded in OC code

        /// <summary>
        /// Read a single byte at the specified offset.
        /// </summary>
        /// <param name="offset">Which byte to read</param>
        /// <returns>The data on that offset</returns>
        public async Task<byte> ReadByte(int offset) => (byte)(await Invoke("readByte", offset))[0];
        /// <summary>
        /// Write a single byte to the specified offset.
        /// </summary>
        /// <param name="offset">Which byte to write data to</param>
        /// <param name="data">The data to write</param>
        public async Task WriteByte(int offset, byte data) => await Invoke("writeByte", offset, data);
        /// <summary>
        /// Read the current contents of the specified sector.
        /// </summary>
        /// <param name="sector">Which sector to read</param>
        /// <returns>The binary data from that sector</returns>
        public async Task<string> ReadSector(int sector) => (await Invoke("readSector", sector))[0];
        /// <summary>
        /// Write the specified contents to the specified sector.
        /// </summary>
        /// <param name="sector">Which sector to write to</param>
        /// <param name="data">The data to write into sector</param>
        public async Task WriteSector(int sector, string data) => await Invoke("readSector", sector, data);
        /// <returns>The number of platters in the drive.</returns>
        public async Task<int> GetPlatterCount() => _platterCount ??= (await Invoke("getPlatterCount"))[0];
        /// <returns>The total capacity of the drive, in bytes.</returns>
        public async Task<int> GetCapacity() => _capacity ??= (await Invoke("getCapacity"))[0];
        /// <returns>The current label of the drive.</returns>
        public async Task<string> GetLabel() => _label ??= (await Invoke("getLabel"))[0];
        /// <summary>
        /// Sets the label of the drive.
        /// </summary>
        /// <param name="label">New drive label</param>
        /// <returns>The new value, which may be truncated.</returns>
        public async Task<string> SetLabel(string label) => _label = (await Invoke("setLabel", label))[0];
#if ROS_PROPERTIES
        public int PlatterCount => GetPlatterCount().Result;
        public int Capacity => GetCapacity().Result;
        public string Label
        {
            get => GetLabel().Result;
            set => SetLabel(value);
        }
#endif
    }
}
