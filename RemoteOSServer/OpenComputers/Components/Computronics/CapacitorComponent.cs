using RemoteOS.Helpers;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("rack_capacitor")]
    public partial class CapacitorComponent : Component
    {
        private int? _maxEnergy;

        public CapacitorComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <returns>The amount of energy stored in this capacitor.</returns>
        public async Task<int> GetEnergy() => (await Invoke("energy"))[0];

        /// <returns>The total amount of energy this capacitor can store.</returns>
        public async Task<int> GetMaxEnergy() => _maxEnergy ??= (await Invoke("maxEnergy"))[0];

#if ROS_PROPERTIES
        public int MaxEnergy => GetMaxEnergy().Result;

#if ROS_PROPS_UNCACHED
        public int Energy => GetEnergy().Result;
#endif
        
#endif
    }
}