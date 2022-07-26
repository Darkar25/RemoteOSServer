namespace RemoteOS.OpenComputers.Components
{
    [Component("chunkloader")]
    public class ChunkLoaderComponent : Component
    {
        public ChunkLoaderComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        bool? _active;
        /// <returns>Whether the chunkloader is currently active.</returns>
        public async Task<bool> IsActive() => _active ??= (await Invoke("isActive"))[0];
        /// <summary>
        /// Enables or disables the chunkloader
        /// </summary>
        /// <param name="state">New chunkloader state</param>
        public async Task SetActive(bool state) => _active = (await Invoke("setActive", state))[0];

#if ROS_PROPERTIES
        public bool Active
        {
            get => IsActive().Result;
            set => SetActive(value);
        }
#endif
    }
}
