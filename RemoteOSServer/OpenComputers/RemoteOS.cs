﻿#define ComponentList
#define InventoryController
#define Drone
#define Hologram
#define GraphicsCard

using EasyJSON;
using RemoteOS.OpenComputers.Components;
using RemoteOS.OpenComputers.Data;
using System.Globalization;
using System.Numerics;

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

        public static async Task<bool> IsExternal(this Component component) => await component.GetSlot() == -1;

        public static async Task<DeviceInfo> GetDeviceInfo(this Component component) => (await component.Parent.Computer.GetDeviceInfo()).TryGetValue(component.Address, out var ret) ? ret : default;

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
        #region GraphicsCard
#if GraphicsCard

        /// <summary>
        /// Gets the max resolution that this graphics card can ever support.
        /// Keep in mind that this method unsafely allocates a buffer to determine the resolution, and if it encounters an error you may end up with invalid data in cache
        /// </summary>
        /// <param name="gpu">Graphics card to analyze</param>
        /// <returns>Maximum resolution supported by this GPU</returns>
        /// <exception cref="OutOfMemoryException">The gpu does not have neough memory</exception>
        /// <exception cref="InvalidOperationException">The buffer allocation/closing failed</exception>
        public static async Task<(int Width, int Height)> GetHardwareMaxResolution(this GraphicsCardComponent gpu)
        {
#if ROS_GLOBAL_CACHING
            var tier = await gpu.GetTier();
            if (GlobalCache.gpuMaxWidth.TryGetValue(tier, out var w) && GlobalCache.gpuMaxHeight.TryGetValue(tier, out var h))
                return (w, h);
#endif
            var bufAlloc = await gpu.Invoke("allocateBuffer");
            if (bufAlloc[0].IsNull) throw new OutOfMemoryException("Not enough video memory.");
            var res = await gpu.Invoke("getBufferSize", bufAlloc[0].AsInt);
            if (res[0].IsNull) throw new InvalidOperationException("Failed to allocate the buffer.");
            if(!(await gpu.Invoke("freeBuffer", bufAlloc[0].AsInt))[0]) throw new InvalidOperationException("Failed to close the buffer.");
#if ROS_GLOBAL_CACHING
            GlobalCache.gpuMaxWidth[tier] = res[0];
            GlobalCache.gpuMaxHeight[tier] = res[1];
#endif
            return (res[0], res[1]);
        }

#endif
        #endregion
        #region Hologram
#if Hologram

        public static async Task<bool> SetRotationAngle(this HologramComponent holo, float x, float y, float z) => await holo.SetRotationAngle(Quaternion.CreateFromYawPitchRoll(x, y, z));

        // This rotation method is still not quite right, needs some tweaking to make it rotate the exact angles needed...
        public static async Task<bool> SetRotationAngle(this HologramComponent holo, Quaternion rot)
        {
            double angle_rad = Math.Acos(rot.W) * 2d;
            double angle_deg = angle_rad * 180 / Math.PI;
            var den = MathF.Sqrt(1.0f - (rot.W * rot.W));
            double x1, y1, z1;
            if (den > 0.0001f)
            {
                x1 = rot.X / den;
                y1 = rot.Y / den;
                z1 = rot.Z / den;
            }
            else
            {
                x1 = Vector3.UnitX.X;
                y1 = Vector3.UnitX.Y;
                z1 = Vector3.UnitX.Z;
            }
            return await holo.SetRotation((float)angle_deg, (float)x1, (float)y1, (float)z1);
        }

#endif
        #endregion
        #region ComponentTiers

        public static async Task<Tier> GetTier(this DataComponent data) => (Tier)(await data.Parent.Execute($"({await data.GetHandle()}.ecdh and 2) or ({await data.GetHandle()}.random and 1) or 0"))[0].AsInt;
        public static async Task<Tier> GetTier(this GraphicsCardComponent gpu)
        {
            var d = (await gpu.GetDeviceInfo()).Width;
            return d == 1 ? Tier.One : d == 4 ? Tier.Two : Tier.Three; // Hardcoded maximum depth
        }
        public static async Task<Tier> GetTier(this HologramComponent holo) => (Tier)await holo.GetMaxDepth() - 1;
        public static async Task<Tier> GetTier(this ScreenComponent screen)
        {
            var d = (await screen.GetDeviceInfo()).Width;
            return d == 1 ? Tier.One : d == 4 ? Tier.Two : Tier.Three; // Hardcoded maximum depth
        }
        public static async Task<Tier> GetTier(this ComputerComponent comp)
        {
            var s = (await comp.GetDeviceInfo()).Capacity;
            return s == 7 ? Tier.One : s == 8 ? Tier.Two : Tier.Three; // Hardcoded inventory sizes, unfortunately creative cases and 3-tier cases are indistinguishable from each other
        }
        public static async Task<Tier> GetTier(this ModemComponent modem)
        {
            var ver = (await modem.GetDeviceInfo()).Version;
            return (Tier)int.Parse(ver[..ver.IndexOf('.')]) - 1;
        }

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
