using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Obsolete("Store all data on the server, not on the remote machine.")]
    [Component("drive")]
    public partial class DriveComponent : Component
    {
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
        public async Task<byte> ReadByte(int offset) => (byte)(await GetInvoker()(offset))[0];

        /// <summary>
        /// Write a single byte to the specified offset.
        /// </summary>
        /// <param name="offset">Which byte to write data to</param>
        /// <param name="data">The data to write</param>
        public partial Task WriteByte(int offset, byte data);

        /// <summary>
        /// Read the current contents of the specified sector.
        /// </summary>
        /// <param name="sector">Which sector to read</param>
        /// <returns>The binary data from that sector</returns>
        public partial Task<string> ReadSector(int sector);

        /// <summary>
        /// Write the specified contents to the specified sector.
        /// </summary>
        /// <param name="sector">Which sector to write to</param>
        /// <param name="data">The data to write into sector</param>
        public partial Task WriteSector(int sector, string data);

        /// <returns>The number of platters in the drive.</returns>
        public async Task<int> GetPlatterCount() => _platterCount ??= (await GetInvoker()())[0];

        /// <returns>The total capacity of the drive, in bytes.</returns>
        public async Task<int> GetCapacity() => _capacity ??= (await GetInvoker()())[0];

        /// <returns>The current label of the drive.</returns>
        public async Task<string> GetLabel() => _label ??= (await GetInvoker()())[0];

        /// <summary>
        /// Sets the label of the drive.
        /// </summary>
        /// <param name="label">New drive label</param>
        /// <returns>The new value, which may be truncated.</returns>
        public async Task<string> SetLabel(string label) => _label = (await GetInvoker()(label))[0];

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
