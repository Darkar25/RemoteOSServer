namespace RemoteOS.OpenComputers.Components
{
    [Component("crafting")]
    public class CraftingComponent : Component
    {
        public CraftingComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        public async Task<bool> Craft() => (await Invoke("craft"))[0];
        public async Task<bool> Craft(int count) => (await Invoke("craft", count))[0];
    }
}
