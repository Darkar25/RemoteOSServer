using RemoteOS.OpenComputers.Data;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("leash")]
    [Tier(Tier.One)]
    public partial class LeashComponent : Component
    {
        public LeashComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Tries to put an entity on the specified side of the device onto a leash.
        /// </summary>
        /// <param name="side">The side to leash from</param>
        /// <returns>true if entity was leashed</returns>
        public partial Task<bool> Leash(Sides side);

        /// <summary>
        /// Unleashes all currently leashed entities.
        /// </summary>
        public partial Task Unleash();
    }
}
