using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("relay")]
    public partial class RelayComponent : Component
    {
        public RelayComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <returns>The signal strength (range) used when relaying messages.</returns>
        public partial Task<int> GetStrength();

        /// <summary>
        /// Set the signal strength (range) used when relaying messages.
        /// </summary>
        /// <param name="strength">New strength value</param>
        public partial Task SetStrength(int strength);

        /// <returns>Whether the access point currently acts as a repeater (resend received wireless packets wirelessly).</returns>
        public partial Task<bool> IsRepeater();

        /// <summary>
        /// Set whether the access point should act as a repeater.
        /// </summary>
        /// <param name="repeater">Repeater mode</param>
        public partial Task SetRepeater(bool repeater);

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public int Range
        {
            get => GetStrength().Result;
            set => SetStrength(value);
        }

        public bool Repeater
        {
            get => IsRepeater().Result;
            set => SetRepeater(value);
        }
#endif
    }
}
