#define ComponentList
#define Component
#define InventoryController
#define Drone
#define Hologram

using EasyJSON;
using RemoteOS.OpenComputers.Components;
using RemoteOS.OpenComputers.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RemoteOS.OpenComputers
{
    public static class RemoteOS
    {
        #region ComponentList
#if ComponentList
        public static bool TryGet<T>(this ComponentList components, out T component) where T : Component => (component = components.GetPrimary<T>()) != null;
#endif
        #endregion
        #region Component
#if Component
        public static async Task<bool> IsExternal(this Component component) => await component.GetSlot() == -1;

        public static async Task<DeviceInfo> GetDeviceInfo(this Component component) => (await component.Parent.Computer.GetDeviceInfo()).TryGetValue(component.Address, out var ret) ? ret : default;
#endif
        #endregion
        #region InventoryController
#if InventoryController
        public static async Task<IEnumerable<ItemStackInfo>> GetInventory(this InventoryContollerComponent inv)
        {
            var invsize = inv.Parent.Components.TryGet<Agent>(out var r) ? await r.GetInventorySize() : 0;
            var res = JSON.Parse(await inv.Parent.RawExecute($"j = {{}} for i = 1, {invsize} do j[i] = {await inv.GetHandle()}.getStackInInternalSlot(i) end return json.encode(j)"));
            return res.Linq.Select(x => new ItemStackInfo(x));
        }

        public static async Task<IEnumerable<ItemStackInfo>> GetInventory(this InventoryContollerComponent inv, Sides side)
        {
            var invsize = await inv.GetInventorySize(side);
            var res = JSON.Parse(await inv.Parent.RawExecute($"j = {{}} for i = 1, {invsize} do j[i] = {await inv.GetHandle()}.getStackInSlot({side.Luaify()}, i) end return json.encode(j)"));
            return res.Linq.Select(x => new ItemStackInfo(x));
        }
#endif
        #endregion
        #region Drone
#if Drone

        public static async Task MoveSync(this DroneComponent comp, double dx, double dy, double dz)
        {
            await comp.Parent.RawExecute($"{await comp.GetHandle()}.move({dx.Luaify()},{dy.Luaify()},{dz.Luaify()}) while({await comp.GetHandle()}.getVelocity() > .01) do end");
        }

#endif
        #endregion
        #region Hologram
#if Hologram

        public static async Task<bool> SetRotationAngle(this HologramComponent holo, float x, float y, float z) => await holo.SetRotationAngle(Quaternion.CreateFromYawPitchRoll(x, y, z));

        public static async Task<bool> SetRotationAngle(this HologramComponent holo, Quaternion rot)
        {
            double angle_rad = Math.Acos(rot.W) * 2;
            double angle_deg = angle_rad * 180 / Math.PI;
            double x1 = rot.X / Math.Sin(angle_rad / 2);
            double y1 = rot.Y / Math.Sin(angle_rad / 2);
            double z1 = rot.Z / Math.Sin(angle_rad / 2);
            return await holo.SetRotation((float)angle_deg, (float)x1, (float)y1, (float)z1);
        }

#endif
        #endregion

        public static string Luaify(this object data) => data switch {
            Sides s when data is Sides => ((int)s).ToString(),
            bool b when data is bool => b ? "true" : "false",
            IFormattable f when data is IFormattable => f.ToString(null, CultureInfo.InvariantCulture),
            IEnumerable<object> a when data is IEnumerable<object> => "{" + string.Join(",", a.Select(x => x.Luaify())) + "}",
            _ when data is null => "nil",
            _ => data.ToString()
        };
    }
}
