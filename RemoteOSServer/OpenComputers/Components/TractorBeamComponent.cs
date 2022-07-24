namespace RemoteOS.OpenComputers.Components
{
    [Component("tractor_beam")]
    public class TractorBeamComponent : Component
    {
        public TractorBeamComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Tries to pick up a random item in the robots' vicinity.
        /// </summary>
        /// <returns>true if a item was picked up</returns>
        public async Task<bool> Suck() => (await Invoke("suck"))[0];
    }
}
