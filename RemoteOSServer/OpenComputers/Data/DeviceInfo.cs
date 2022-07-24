namespace RemoteOS.OpenComputers.Data
{
    public struct DeviceInfo
    {
        /// <summary>
        /// device's class, e.g. <see cref="DeviceClass.Processor"/>
        /// </summary>
        public DeviceClass Class;
        /// <summary>
        /// human-readable description of the hardware node, e.g. "Ethernet interface"
        /// </summary>
        public string Description;
        /// <summary>
        /// vendor/manufacturer of the device, e.g. "Minecorp Inc."
        /// </summary>
        public string Vendor;
        /// <summary>
        /// product name of the device, e.g. "ATY Raderps 4200X"
        /// </summary>
        public string Product;
        /// <summary>
        /// version/release of the device, e.g. "2.1.0"
        /// </summary>
        public string Version;
        /// <summary>
        /// serial number of the device
        /// </summary>
        public string Serial;
        /// <summary>
        /// maximum capacity reported by the device, e.g. unformatted size of a disk
        /// </summary>
        public int Capacity;
        /// <summary>
        /// actual size of the device, e.g. actual usable space on a disk
        /// </summary>
        public int Size;
        /// <summary>
        /// bus clock (in Hz) of the device, e.g. call speed(s) of a component
        /// </summary>
        public IEnumerable<int> Clock;
        /// <summary>
        /// address width of the device, in the broadest sense
        /// </summary>
        public int Width;
    }
}
