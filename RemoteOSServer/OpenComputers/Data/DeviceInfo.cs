namespace RemoteOS.OpenComputers.Data
{
    public struct DeviceInfo
    {
        public DeviceClass Class;
        public string Description;
        public string Vendor;
        public string Product;
        public string Version;
        public string Serial;
        public int Capacity;
        public int Size;
        public IEnumerable<int> Clock;
        public int Width;
    }
}
