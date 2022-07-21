namespace RemoteOS.OpenComputers.Components
{
    [Component("waypoint")]
    public class WaypointComponent : Component
    {
        public WaypointComponent(Machine parent, Guid address) : base(parent, address)
        {
        }
        public async Task<string> GetLabel() => (await Invoke("getLabel"))[0];
        public async Task SetLabel(string label) => await Invoke("setLabel", @$"""{label}""");

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public string Label
        {
            get => GetLabel().Result;
            set => SetLabel(value);
        }
#endif
    }
}
