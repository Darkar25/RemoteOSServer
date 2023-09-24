using RemoteOS.OpenComputers;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Obsolete("Store all data on the server, not on the remote machine.")]
    [Component("eeprom")]
    public partial class EEPROMComponent : Component
    {
        string? _bytes;
        string? _label;
        int? _size;
        int? _dataSize;
        string? _data;

        public EEPROMComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Make this EEPROM readonly if it isn't already. This process cannot be reversed!
        /// </summary>
        /// <param name="checksum"></param>
        /// <returns></returns>
        public partial Task<bool> MakeReadonly(string checksum);

        /// <returns>The currently stored byte array.</returns>
        public async Task<string> Get() => _bytes ??= (await GetInvoker()())[0];

        /// <summary>
        /// Overwrite the currently stored byte array.
        /// </summary>
        /// <param name="bytes">New data byte array</param>
        /// <exception cref="IOException">The EEPROM cannot contain that much data</exception>
        public async Task Set(string bytes)
        {
            if (bytes.Length > await GetSize()) throw new IOException("Not enough disk space");
            if((await GetInvoker()(bytes))[1].IsNull)
                _bytes = bytes;
        }

        /// <returns>The label of the EEPROM.</returns>
        public async Task<string> GetLabel() => _label ??= (await GetInvoker()())[0];

        /// <summary>
        /// Set the label of the EEPROM.
        /// </summary>
        /// <param name="label">The new label</param>
        public async Task SetLabel(string label)
        {
            var res = await GetInvoker()(label);
            if (res[1].IsNull)
                _label = res[0];
        }

        /// <returns>The storage capacity of this EEPROM.</returns>
        public async Task<int> GetSize() => _size ??=
#if ROS_GLOBAL_CACHING
            GlobalCache.eepromSize ??=
#endif
            (await GetInvoker()())[0];

        /// <returns>The storage capacity of this EEPROM.</returns>
        public async Task<int> GetDataSize() => _dataSize ??=
#if ROS_GLOBAL_CACHING
            GlobalCache.eepromDataSize ??=
#endif
            (await GetInvoker()())[0];

        /// <returns>The currently stored byte array.</returns>
        public async Task<string> GetData() => _data ??= (await GetInvoker()());

        /// <summary>
        /// Overwrite the currently stored byte array.
        /// </summary>
        /// <param name="data">New data byte array</param>
        /// <exception cref="IOException">The EEPROM cannot contain that much data</exception>
        public async Task SetData(string data)
        {
            if(data.Length > await GetDataSize()) throw new IOException("Not enough disk space");
            if ((await GetInvoker()(data))[1].IsNull)
                _data = data;
        }

        /// <returns>The checksum of the data on this EEPROM.</returns>
        public partial Task<string> GetChecksum();

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
