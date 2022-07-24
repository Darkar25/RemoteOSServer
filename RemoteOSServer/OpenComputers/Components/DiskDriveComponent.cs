namespace RemoteOS.OpenComputers.Components
{
    [Component("disk_drive")]
    public class DiskDriveComponent : Component
    {
        public DiskDriveComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <returns>Whether some medium is currently in the drive.</returns>
        public async Task<bool> IsEmpty() => (await Invoke("isEmpty"))[0];
        /// <summary>
        /// Eject the currently present medium from the drive.
        /// </summary>
        /// <param name="velocity">The velocity to eject with</param>
        /// <returns>true if something was ejected</returns>
        public async Task<bool> Eject(float velocity = 0) => (await Invoke("eject", velocity))[0];
        /// <returns>The internal floppy disk address</returns>
        public async Task<string> GetMedia() => (await Invoke("media"))[0];


#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public bool Empty => IsEmpty().Result;
        public string Media => GetMedia().Result;
#endif
    }
}
