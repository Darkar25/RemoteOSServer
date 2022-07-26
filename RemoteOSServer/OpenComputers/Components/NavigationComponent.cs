﻿using RemoteOS.OpenComputers.Data;
using System.Numerics;

namespace RemoteOS.OpenComputers.Components
{
    [Component("navigation")]
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
            var res = await Invoke("findWaypoints", range);
            List<WaypointInfo> ret = new();
            foreach (var w in res.Values)
                ret.Add(new()
                {
                    Label = w["label"],
                    RedstoneLevel = w["redstone"],
                    RelativePosition = new(w["position"][0], w["position"][1], w["position"][2])
                });
            return ret;
        }
        /// <returns>The current relative position of the robot.</returns>
        public async Task<(bool Success, string Reason, Vector3 Position)> GetPosition()
        {
            var res = await Invoke("getPosition");
            if (res[0].IsNull)
                return (false, res[1], default);
            else
                return (true, "", new(res[0], res[1], res[2]));
        }
        /// <returns>The current orientation of the robot.</returns>
        public async Task<Sides> GetFacing() => (Sides)(await Invoke("getFacing"))[0].AsInt;
        /// <returns>The operational range of the navigation upgrade.</returns>
        public async Task<int> GetRange() => _range ??= (await Invoke("getRange"))[0];

#if ROS_PROPERTIES
#if ROS_PROPS_UNCACHED
        public (bool Success, string Reason, Vector3 Position) Position => GetPosition().Result;

        public Sides Facing => GetFacing().Result;
#endif

        public int Range => GetRange().Result;
#endif
    }
}
