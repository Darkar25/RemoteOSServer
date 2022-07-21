namespace RemoteOS.OpenComputers.Components
{
    [Component("disk_drive")]
    public class DiskDriveComponent : Component
    {
        public DiskDriveComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        public async Task<bool> IsEmpty() => (await Invoke("isEmpty"))[0];
        public async Task<bool> Eject(float velocity = 0) => (await Invoke("eject", velocity))[0];
        public async Task<string> GetMedia() => (await Invoke("media"))[0];


#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public bool Empty => IsEmpty().Result;
        public string Media => GetMedia().Result;
#endif
    }
}
