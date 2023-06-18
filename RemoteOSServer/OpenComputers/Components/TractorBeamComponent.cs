using RemoteOS.OpenComputers.Data;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("tractor_beam")]
    [Tier(Tier.Three)]
    public partial class TractorBeamComponent : Component
    {
        public TractorBeamComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

		/// <summary>
		/// Tries to pick up a random item in the robots' vicinity.
		/// </summary>
		/// <returns>true if a item was picked up</returns>
		public partial Task<bool> Suck();
    }
}
