using OneOf;
using RemoteOS.OpenComputers.Data;
using System.Numerics;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("navigation")]
    [Tier(Tier.Two)]
    public class NavigationComponent : Component
    {
        public NavigationComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        int? _range;

        /// <summary>
        /// Find waypoints in the specified range.
        /// </summary>
        /// <param name="range">Range for searching waypoints</param>
        /// <returns>A list of found waypoints</returns>
        public async Task<IEnumerable<WaypointInfo>> GetWaypoints(int range)
        {
            return (await Invoke("findWaypoints", range)).Linq.Select(x => new WaypointInfo
            {
				Label = x.Value["label"],
				RedstoneLevel = x.Value["redstone"],
				RelativePosition = new(x.Value["position"][0], x.Value["position"][1], x.Value["position"][2])
			});
        }

        /// <returns>The current relative position of the robot or the reason why it could not be retrieved.</returns>
        public async Task<ReasonOr<Vector3>> GetPosition()
        {
            var res = await GetInvoker()();
            if (res[0].IsNull)
                return new Reason(res[1].Value);
            else
                return new Vector3(res[0], res[1], res[2]);
        }

        /// <returns>The current orientation of the robot.</returns>
        public async Task<Sides> GetFacing() => (Sides)(await GetInvoker()())[0].AsInt;

        /// <returns>The operational range of the navigation upgrade.</returns>
        public async Task<int> GetRange() => _range ??= (await GetInvoker()())[0];

#if ROS_PROPERTIES
#if ROS_PROPS_UNCACHED
        public ReasonOr<Vector3> Position => GetPosition().Result;

        public Sides Facing => GetFacing().Result;
#endif

		public int Range => GetRange().Result;
#endif
    }
}
