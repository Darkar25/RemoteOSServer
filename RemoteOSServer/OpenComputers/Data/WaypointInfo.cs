using System.Numerics;

namespace RemoteOS.OpenComputers.Data
{
    public struct WaypointInfo
    {
        /// <summary>
        /// Label of the waypoint
        /// </summary>
        public string Label;
        /// <summary>
        /// Redstone strength of this waypoint
        /// </summary>
        public int RedstoneLevel;
        /// <summary>
        /// Relative position to this waypoint
        /// </summary>
        public Vector3 RelativePosition;
    }
}
