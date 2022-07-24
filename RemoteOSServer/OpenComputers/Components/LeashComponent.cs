namespace RemoteOS.OpenComputers.Components
{
    [Component("leash")]
    public class LeashComponent : Component
    {
        public LeashComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Tries to put an entity on the specified side of the device onto a leash.
        /// </summary>
        /// <param name="side">The side to leash from</param>
        /// <returns>true if entity was leashed</returns>
        public async Task<bool> Leash(Sides side) => (await Invoke("leash", side))[0];
        /// <summary>
        /// Unleashes all currently leashed entities.
        /// </summary>
        public async Task Unleash() => await Invoke("unleash");
    }
}
