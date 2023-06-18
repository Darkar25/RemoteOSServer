using RemoteOS.Helpers;
using RemoteOS.OpenComputers.Data;

namespace RemoteOS.OpenComputers.Components
{
    [Component("chunkloader")]
    [Tier(Tier.Three)]
    public class ChunkLoaderComponent : Component
    {
        public ChunkLoaderComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        bool? _active;
        
        /// <returns>Whether the chunkloader is currently active.</returns>
        public async Task<bool> IsActive() => _active ??= await InvokeFirst("isActive");

        /// <summary>
        /// Enables or disables the chunkloader
        /// </summary>
        /// <param name="state">New chunkloader state</param>
        public async Task SetActive(bool state) => _active = await InvokeFirst("setActive", state);

#if ROS_PROPERTIES
        public bool Active
        {
            get => IsActive().Result;
            set => SetActive(value);
        }
#endif
    }
}
