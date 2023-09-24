using OneOf;
using OneOf.Types;
using RemoteOS.OpenComputers.Data;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("crafting")]
    [Tier(Tier.Two)]
    public class CraftingComponent : Component
    {
        public CraftingComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Tries to craft the specified number of items in the top left area of the inventory.
        /// </summary>
        /// <param name="count">How many items to craft</param>
        /// <returns>How many items were crafter or an error</returns>
        public async Task<OneOf<int, Error>> Craft(int count = 64)
        {
            var res = await GetInvoker()(count);
			if (!res[0]) return new Error();
			return res[1].AsInt;
        }
    }
}
