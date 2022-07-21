namespace RemoteOS.OpenComputers.Components
{
    [Component("tractor_beam")]
    public class TractorBeamComponent : Component
    {
        public TractorBeamComponent(Machine parent, Guid address) : base(parent, address)
        {
        }
        public async Task<bool> Suck() => (await Invoke("suck"))[0];
    }
}
