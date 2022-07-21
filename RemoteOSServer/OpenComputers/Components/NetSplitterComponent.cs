namespace RemoteOS.OpenComputers.Components
{
    [Component("net_splitter")]
    public class NetSplitterComponent : Component
    {
        public NetSplitterComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        public async Task<bool> Open(Sides side) => (await Invoke("open", side))[0];
        public async Task<bool> Close(Sides side) => (await Invoke("close", side))[0];
        public async Task<bool[]> GetSides() => (await Invoke("getSides"))[0].Linq.Select(x => x.Value.AsBool).ToArray();
        public async Task SetSides(bool[] sides) => await Invoke("setSides", new object[] { sides });

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public bool[] Sides
        {
            get => GetSides().Result;
            set => SetSides(value);
        }
#endif
    }
}
