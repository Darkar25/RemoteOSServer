using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("access_point")]
    public partial class AccessPointComponent : Component
    {
        public AccessPointComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <returns>The signal strength (range) used when relaying messages.</returns>
        public partial Task<int> GetStrength();

        /// <summary>
        /// Set the signal strength (range) used when relaying messages.
        /// </summary>
        /// <param name="strength">New strength</param>
        public partial Task SetStrength(int strength);

        /// <returns>Whether the access point currently acts as a repeater (resend received wireless packets wirelessly).</returns>
        public partial Task<bool> IsRepeater();

        /// <summary>
        /// Set whether the access point should act as a repeater.
        /// </summary>
        /// <param name="repeater">Should access point act as repeater</param>
        /// <returns></returns>
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
