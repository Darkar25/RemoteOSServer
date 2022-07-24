namespace RemoteOS.OpenComputers.Components
{
    [Component("net_splitter")]
    public class NetSplitterComponent : Component
    {
        public NetSplitterComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Open the side
        /// </summary>
        /// <param name="side">The side to open</param>
        /// <returns>true if it changed to open.</returns>
        public async Task<bool> Open(Sides side) => (await Invoke("open", side))[0];
        /// <summary>
        /// Close the side,
        /// </summary>
        /// <param name="side">Side to close</param>
        /// <returns>true if it changed to close.</returns>
        public async Task<bool> Close(Sides side) => (await Invoke("close", side))[0];
        /// <returns>Current open/close state of all sides in an array, indexed by direction.</returns>
        public async Task<bool[]> GetSides() => (await Invoke("getSides"))[0].Linq.Select(x => x.Value.AsBool).ToArray();
        /// <summary>
        /// Set open state (true/false) of all sides in an array; index by direction.
        /// </summary>
        /// <param name="sides">The array of sides</param>
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
