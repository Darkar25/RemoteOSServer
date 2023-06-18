using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("waypoint")]
    public partial class WaypointComponent : Component
    {
        public WaypointComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <returns>The current label of this waypoint.</returns>
        public partial Task<string> GetLabel();

        /// <summary>
        /// Set the label for this waypoint.
        /// </summary>
        /// <param name="label">New label</param>
        public partial Task SetLabel(string label);

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public string Label
        {
            get => GetLabel().Result;
            set => SetLabel(value);
        }
#endif
    }
}
