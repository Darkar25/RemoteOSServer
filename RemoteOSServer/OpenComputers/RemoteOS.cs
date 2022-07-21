#define Component
#define InventoryController
#define Drone

using EasyJSON;
using RemoteOS.OpenComputers.Components;
using RemoteOS.OpenComputers.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteOS.OpenComputers
{
    public static class RemoteOS
    {
        #region Component
#if Component
        public static bool TryGet<T>(this ComponentList components, out T component) where T : Component => (component = components.GetPrimary<T>()) != null;
#endif
        #endregion
        #region InventoryController
#if InventoryController
        public static async Task<IEnumerable<ItemStackInfo>> GetInventory(this InventoryContollerComponent inv)
        {
            var invsize = inv.Parent.Components.TryGet<Agent>(out var r) ? await r.GetInventorySize() : 0;
            var res = JSON.Parse(await inv.Parent.RawExecute($"j = {{}} for i = 1, {invsize} do j[i] = {inv.Handle}.getStackInInternalSlot(i) end return json.encode(j)"));
            return res.Linq.Select(x => new ItemStackInfo(x));
        }

        public static async Task<IEnumerable<ItemStackInfo>> GetInventory(this InventoryContollerComponent inv, Sides side)
        {
            var invsize = await inv.GetInventorySize(side);
            var res = JSON.Parse(await inv.Parent.RawExecute($"j = {{}} for i = 1, {invsize} do j[i] = {inv.Handle}.getStackInSlot({side.Luaify()}, i) end return json.encode(j)"));
            return res.Linq.Select(x => new ItemStackInfo(x));
        }
#endif
        #endregion
        #region Drone
#if Drone

        public static async Task MoveSync(this DroneComponent comp, double dx, double dy, double dz)
        {
            await comp.Parent.RawExecute($"{comp.Handle}.move({dx.Luaify()},{dy.Luaify()},{dz.Luaify()}) while({comp.Handle}.getVelocity() > .01) do end");
        }

#endif
        #endregion

        public static string Luaify(this object data)
        {
            return data switch
            {
                Sides s when data is Sides => ((int)s).ToString(),
                bool b when data is bool => b ? "true" : "false",
                IFormattable f when data is IFormattable => f.ToString(null, CultureInfo.InvariantCulture),
                IEnumerable<object> a when data is IEnumerable<object> => "{" + string.Join(",", a.Select(x => x.Luaify())) + "}",
                _ when data is null => "nil",
                _ => data.ToString()
            };
        }
    }
}
