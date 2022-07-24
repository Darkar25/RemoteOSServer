namespace RemoteOS.OpenComputers.Components
{
    [Component("crafting")]
    public class CraftingComponent : Component
    {
        public CraftingComponent(Machine parent, Guid address) : base(parent, address)
        {
        }
        /// <summary>
        /// Tries to craft the specified number of items in the top left area of the inventory.
        /// </summary>
        /// <param name="count">How many items to craft</param>
        /// <returns>Whether the craft was successful and how many items were crafted</returns>
        public async Task<(bool Success, int Amount)> Craft(int count = 64)
        {
            var res = await Invoke("craft", count);
            return (res[0], res[1]);
        }
    }
}
