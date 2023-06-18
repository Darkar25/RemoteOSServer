using RemoteOS.OpenComputers.Data;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("sign")]
    [Tier(Tier.One)]
    public partial class SignComponent : Component
    {
        public SignComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <returns>The text on the sign in front of the host.</returns>
        public partial Task<string> GetValue();

        /// <summary>
        /// Set the text on the sign in front of the host.
        /// </summary>
        /// <param name="value">New value</param>
        public partial Task SetValue(string value);

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public string Value
        {
            get => GetValue().Result;
            set => SetValue(value);
        }
#endif
    }
}
