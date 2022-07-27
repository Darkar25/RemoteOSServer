namespace RemoteOS.OpenComputers.Components
{
    [Component("waypoint")]
    public class WaypointComponent : Component
    {
        public WaypointComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <returns>The current label of this waypoint.</returns>
        public async Task<string> GetLabel() => (await Invoke("getLabel"))[0];
        /// <summary>
        /// Set the label for this waypoint.
        /// </summary>
        /// <param name="label">New label</param>
        public async Task SetLabel(string label) => await Invoke("setLabel", label);

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public string Label
        {
            get => GetLabel().Result;
            set => SetLabel(value);
        }
#endif
    }
}
