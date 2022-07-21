namespace RemoteOS.OpenComputers.Components
{
    [Component("leash")]
    public class LeashComponent : Component
    {
        public LeashComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        public async Task<bool> Leash(Sides side) => (await Invoke("leash", side))[0];
        public async Task Unleash() => await Invoke("unleash");
    }
}
