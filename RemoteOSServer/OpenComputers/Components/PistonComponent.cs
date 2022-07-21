namespace RemoteOS.OpenComputers.Components
{
    [Component("piston")]
    public class PistonComponent : Component
    {
        public PistonComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        public async Task<bool> Push(Sides side) => (await Invoke("push", side))[0];
    }
}
