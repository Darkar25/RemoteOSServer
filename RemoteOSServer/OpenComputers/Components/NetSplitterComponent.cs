using RemoteOS.Helpers;
using RemoteOS.OpenComputers.Data;

namespace RemoteOS.OpenComputers.Components
{
    [Component("net_splitter")]
    public partial class NetSplitterComponent : Component
    {
        public NetSplitterComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Open the side
        /// </summary>
        /// <param name="side">The side to open</param>
        /// <returns>true if it changed to open.</returns>
        public partial Task<bool> Open(Sides side);

        /// <summary>
        /// Close the side,
        /// </summary>
        /// <param name="side">Side to close</param>
        /// <returns>true if it changed to close.</returns>
        public partial Task<bool> Close(Sides side);

        /// <returns>Current open/close state of all sides in an array, indexed by direction.</returns>
        public async Task<bool[]> GetSides() => (await InvokeFirst("getSides")).Linq.Select(x => x.Value.AsBool).ToArray();

        /// <summary>
        /// Set open state (true/false) of all sides in an array; index by direction.
        /// </summary>
        /// <param name="sides">The array of sides</param>
        public Task SetSides(bool[] sides) => Invoke("setSides", new object[] { sides });

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public bool[] Sides
        {
            get => GetSides().Result;
            set => SetSides(value);
        }
#endif
    }
}
