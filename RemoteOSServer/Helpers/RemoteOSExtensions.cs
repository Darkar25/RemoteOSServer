#define ComponentList
#define InventoryController
#define Drone
#define Hologram
#define GraphicsCard
#define Robot
#define Geolyzer

using EasyJSON;
using OneOf;
using OneOf.Types;
using RemoteOS.OpenComputers;
using RemoteOS.OpenComputers.Components;
using RemoteOS.OpenComputers.Data;
using RemoteOS.OpenComputers.Exceptions;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace RemoteOS.Helpers
{
    public static class RemoteOSExtensions
    {
        #region ComponentList
#if ComponentList

        /// <summary>
        /// Tries to get the component
        /// </summary>
        /// <typeparam name="T">Which component to get</typeparam>
        /// <param name="components">The component list to get the component from</param>
        /// <returns>true if this component available, false otherwise</returns>
        public static bool TryGet<T>(this ComponentList components, out T component) where T : Component => (component = components.GetPrimary<T>()) is not null;

#endif
        #endregion
        #region Component

        /// <param name="component">The component</param>
        /// <returns>Whether this component is external</returns>
        public static async Task<bool> IsExternal(this Component component) => await component.GetSlot() == -1;

        /// <param name="component">The component</param>
        /// <returns>The component`s device info</returns>
        public static async Task<DeviceInfo> GetDeviceInfo(this Component component) => (await component.Parent.Computer.GetDeviceInfo()).TryGetValue(component.Address, out var ret) ? ret : default;
        
        /// <summary>
        /// Hook to the specified method to intercept its execution and/or modify the return value
        /// <br>Be careful with this method. You may break the server if you do something wrong</br>
        /// </summary>
        /// <remarks>
        /// Function argument passes the original method call, you can use this in your hook code to call the original method and get its return value
        /// </remarks>
        /// <param name="component">Which component to hook</param>
        /// <param name="method">Whick method to hook</param>
        /// <param name="code">The hook code</param>
        public static async Task Hook(this Component component, string method, Func<string, string> code)
        {
            var hooked = $"{await component.GetHandle()}.{method}";
            var original = $"global.hook[\"{hooked}\"]";
            await component.Parent.RawExecute($"global.hook=global.hook or{{}}if {original}==nil then {original}={hooked} {hooked}=function(...) {code(original)} end end");
        }
        
        /// <summary>
        /// Removes the hook from the method
        /// </summary>
        /// <param name="component">The component that had been hooked</param>
        /// <param name="method">The method that had been hooked</param>
        public static async Task Unhook(this Component component, string method)
        {
            var hooked = $"{await component.GetHandle()}.{method}";
            var original = $"global.hook[\"{hooked}\"]";
            await component.Parent.RawExecute($"global.hook=global.hook or{{}}if {original}~=nil then {hooked}={original} {original}=nil end");
        }
		
        /// <summary>
		/// Attaches a hook that sends a signal to the server when its successfully called
		/// </summary>
		/// <param name="component">The component that contains target method</param>
		/// <param name="method">The method that needs to be hooked</param>
		/// <param name="signal">Signal name</param>
		public static Task SignalHook(this Component component, string method, string signal) => component.Hook(method, (original) => $"local res={original}(...)if res then computer.pushSignal(\"{signal}\",...) end return res");
		
        /// <inheritdoc cref="SignalHook(Component, string, string)"/>
		/// <param name="action">Action to perform when signal is received</param>
		public static Task SignalHook(this Component component, string method, Action<JSONArray> action)
		{
			component.Parent.Listen($"ROS:{component.Type}_{method}", action);
			return SignalHook(component, method, $"ROS:{component.Type}_{method}");
		}

		#endregion
		#region InventoryController
#if InventoryController
		public static async Task<IEnumerable<ItemStackInfo>> GetInventory(this InventoryContollerComponent inv)
        {
            var invsize = (await inv.Parent.GetComponents()).TryGet<Agent>(out var r) ? await r.GetInventorySize() : 0;
            return JSON.Parse((await inv.Parent.RawExecute($"j = {{}} for i = 1, {invsize} do j[i] = {await inv.GetHandle()}.getStackInInternalSlot(i) end return json.encode(j)"))).Linq.Select(x => ItemStackInfo.FromJson(x));
        }

        public static async Task<IEnumerable<ItemStackInfo>> GetInventory(this InventoryContollerComponent inv, Sides side)
        {
            var invsize = await inv.GetInventorySize(side);
            return JSON.Parse((await inv.Parent.RawExecute($"j = {{}} for i = 1, {invsize} do j[i] = {await inv.GetHandle()}.getStackInSlot({side.Luaify()}, i) end return json.encode(j)"))).Linq.Select(x => ItemStackInfo.FromJson(x));
        }
#endif
        #endregion
        #region Drone
#if Drone

        /// <inheritdoc cref="DroneComponent.Move(float, float, float)"/>
        public static async Task MoveSync(this DroneComponent comp, double dx, double dy, double dz)
        {
            await comp.Parent.RawExecute($"{await comp.GetHandle()}.move({dx.Luaify()},{dy.Luaify()},{dz.Luaify()}) while({await comp.GetHandle()}.getVelocity() > .01) do end");
        }
		
        /// <inheritdoc cref="DroneComponent.Move(float, float, float)"/>
		/// <param name="side">Which side to move to</param>
		public static Task MoveSync(this DroneComponent comp, Sides side)
		{
			float dx = 0, dy = 0, dz = 0;
			switch ((Sides)(int)side)
			{
				case Sides.Bottom:
					dy = -1;
					break;
				case Sides.Top:
					dy = 1;
					break;
				case Sides.Back:
					dz = -1;
					break;
				case Sides.Front:
					dz = 1;
					break;
				case Sides.Right:
					dx = -1;
					break;
				case Sides.Left:
					dx = 1;
					break;
			}
			return MoveSync(comp, dx, dy, dz);
		}

		/// <inheritdoc cref="DroneComponent.Move(float, float, float)"/>
		/// <param name="side">Which side to move to</param>
		public static Task Move(this DroneComponent comp, Sides side)
		{
			float dx = 0, dy = 0, dz = 0;
			switch ((Sides)(int)side)
			{
				case Sides.Bottom:
					dy = -1;
					break;
				case Sides.Top:
					dy = 1;
					break;
				case Sides.Back:
					dz = -1;
					break;
				case Sides.Front:
					dz = 1;
					break;
				case Sides.Right:
					dx = -1;
					break;
				case Sides.Left:
					dx = 1;
					break;
			}
			return comp.Move(dx, dy, dz);
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
		/// <exception cref="BufferException">The buffer allocation/closing failed</exception>
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
            if (res[0].IsNull) throw new BufferException("Failed to allocate the buffer.");
            if(!(await gpu.InvokeFirst("freeBuffer", bufAlloc[0].AsInt))) throw new BufferException("Failed to close the buffer.");
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

        /// <summary>
        /// Set the rotation angle for the hologram
        /// </summary>
        /// <param name="holo">Holographic projector</param>
        /// <param name="x">X axis rotation angle in degrees</param>
        /// <param name="y">Y axis rotation angle in degrees</param>
        /// <param name="z">Z axis rotation angle in degrees</param>
        /// <returns>true if rotation was successful</returns>
        public static Task<bool> SetRotationAngle(this HologramComponent holo, float x, float y, float z) => holo.SetRotationAngle(Quaternion.CreateFromYawPitchRoll(x, y, z));

        // This rotation method is still not quite right, needs some tweaking to make it rotate the exact angles needed...
        /// <inheritdoc cref="SetRotationAngle(HologramComponent, float, float, float)"/>
        /// <param name="rot">Rotation quaternion</param>
        public static Task<bool> SetRotationAngle(this HologramComponent holo, Quaternion rot)
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
            return holo.SetRotation((float)angle_deg, (float)x1, (float)y1, (float)z1);
        }

#endif
        #endregion
        #region Robot
#if Robot

        /// <summary>
        /// This method uses <see cref="GeolyzerComponent"/> to detect facing of the robot...The robot need to hold a block.
        /// </summary>
        /// <param name="comp">The robot</param>
        /// <returns>Facing of the robot</returns>
        public static async Task<Sides> NavigationlessGetFacing(this RobotComponent comp)
        {
            if ((await comp.Parent.GetComponents()).TryGet<GeolyzerComponent>(out var geo))
            {
                if (!(await comp.Detect(Sides.Front)).Passable) await comp.Swing(Sides.Front);
                await comp.Turn(false);
                if (!(await comp.Detect(Sides.Front)).Passable) await comp.Swing(Sides.Front);
                await comp.Turn(false);
                if (!(await comp.Detect(Sides.Front)).Passable) await comp.Swing(Sides.Front);
                await comp.Turn(false);
                if ((await comp.Detect(Sides.Front)).Passable) await comp.Place(Sides.Front);
                var res = await geo.Scan(-1, -1, 0, 3, 2, 1, true);
                return res[0, 0, 1] > 0 ? Sides.Right : res[1, 0, 0] > 0 ? Sides.Back : res[2, 0, 1] > 0 ? Sides.Left : Sides.Front;
            }
            throw new MissingComponentException<GeolyzerComponent>();
        }

#endif
        #endregion
        #region Geolyzer
#if Geolyzer

        /// <inheritdoc cref="GeolyzerComponent.Scan(int, int, int, int, int, int, bool)"/>
        public static Task<double[,,]> ScanXYZ(this GeolyzerComponent comp, int x, int y, int z, int width, int height, int depth, bool ignoreReplaceable = false) => comp.Scan(x, z, y, width, depth, height, ignoreReplaceable);

        // Credits: @eu_tomat, https://computercraft.ru/topic/3352-metodika-uskorennogo-vychisleniya-konstanty-shuma-geoskanera/
        /// <summary>
        /// Calculates the noise constant. There has to be a block under the Geolyzer,
        /// </summary>
        /// <remarks>This method is quite expensive, use this only when ROS_GLOBAL_CACHING is turned on!</remarks>
        /// <param name="comp">The geolyzer that have to perform calculations</param>
        /// <returns>The noise constant defined in opencomputers config or the error is it can not be determined</returns>
        public static async Task<OneOf<float, Error>> GetNoiseConstant(this GeolyzerComponent comp)
        {
#if ROS_GLOBAL_CACHING
            if (GlobalCache.geolyzerNoise.HasValue) return GlobalCache.geolyzerNoise.Value;
#endif
            // Minified version of eu_tomat's script
            var res = await comp.Parent.RawExecute(@$"local a={{}}local b=0;local c,d,e,f;local g;for h=1,99 do c={await comp.GetHandle()}.scan(0,0,-1,1,1,1)[1]g=false;if not a[c]then for i,j in pairs(a)do d=math.abs(c-i)if not e or e>d then e=d;g=true end;if not f or f<d then f=d;g=true end end;a[c]=true;b=b+1 end;if g and f/e>128 then break end end;return json.encode(g and f/e>128 and{{true,(e*4224*1e2+0.5)//1/1e2}}or{{false,nil}})");
			var result = JSON.Parse(res);
#if ROS_GLOBAL_CACHING
            if (result[0].AsBool) GlobalCache.geolyzerNoise = result[1];
#endif
            if (!result[0]) return new Error();
            return result[1].AsFloat;
        }

        // Credits: @pengoid @doob @eu_tomat, https://computercraft.ru/topic/3346-vychislenie-pogreshnosti-geoskanera/ and https://computercraft.ru/topic/3950-okkultnye-praktiki-pri-poiske-rudy/
        /// <summary>
        /// Clears the noise of geolyzer scan result
        /// </summary>
        /// <param name="comp">The geolyzer component that was user for the scan</param>
        /// <param name="geoResult">The result of a scan</param>
        /// <param name="offsetX">X offset that was used for scanning</param>
        /// <param name="offsetY">Y offset that was used for scanning</param>
        /// <param name="offsetZ">Z offset that was used for scanning</param>
        /// <param name="targetHardness">The hardness level that needs to be filtered out of everything else</param>
        /// <returns>Boolean matrix: true if the block with target hardness is present on such coordinates, false otherwise, [x, y, z]</returns>
        public static async Task<bool[,,]> DenoiseFilter(this GeolyzerComponent comp, double[,,] geoResult, int offsetX, int offsetY, int offsetZ, float targetHardness)
        {
            bool[,,] res = new bool[geoResult.GetLength(0), geoResult.GetLength(1), geoResult.GetLength(2)];
            var noise = (await comp.GetNoiseConstant()).Match(noise => noise, err => 2f);
            for (int x = 0; x < res.GetLength(0); x++)
                for (int y = 0; y < res.GetLength(1); y++)
                    for (int z = 0; z < res.GetLength(2); z++) {
                        var ox = x + offsetX;
                        var oy = y + offsetY;
                        var oz = z + offsetZ;
                        var a = 4224 * (geoResult[x, y, z] - targetHardness) / Math.Sqrt(ox * ox + oy * oy + oz * oz) / noise;
                        res[x, y, z] = geoResult[x, y, z] != 0 && a > -128 && a < 127 && (a % 1 > 0.9998 || a % 1 < 0.0002);
                    }
            return res;
        }

#endif
        #endregion

        #region Guard Clauses

        public static void ThrowIfOtherThan(this Sides side, params Sides[] validSides)
        {
            if (!validSides.Contains(side)) throw new InvalidSideException(validSides);
        }

        public static void ThrowIfOppositeTo(this Sides side, Sides face)
        {
            if (face == side.Opposite()) throw new InvalidSideException("Face cannot be opposite to the side.");
        }

        #endregion

        /// <summary>
        /// Prepare object for transfer to remote machine
        /// </summary>
        /// <param name="data">The object to transform</param>
        /// <returns>String representation of this object that can be accepted from lua code</returns>
        /// <exception cref="InvalidOperationException">This object cannot be transformed</exception>
        public static string Luaify(this object data) => data switch {
            _ when data is null => "nil",
            JSONObject o when data is JSONObject => o.Linq.ToDictionary(x => x.Key, x => x.Value).Luaify(),
            JSONArray ja when data is JSONArray => ja.Linq.Select(x => x.Value).Luaify(),
            Sides s when data is Sides => ((int)s).Luaify(),
            bool b when data is bool => b ? "true" : "false",
            Guid g when data is Guid => "\"" + g + "\"",
            IFormattable f when data is IFormattable => f.ToString(null, CultureInfo.InvariantCulture),
            IDictionary<object, object> d when data is IDictionary<object, object> => "{" + string.Join(",", d.Select(x => "[" + x.Key.Luaify() + "]=" + x.Value.Luaify())) + "}",
            IEnumerable<object> a when data is IEnumerable<object> => "{" + string.Join(",", a.Select(x => x.Luaify())) + "}",
            string str when data is string => str.ToLiteral(),
            Component c when data is Component => c.Address.Luaify(),
            Color col when data is Color => col.ToArgb().Luaify(),
            _ => throw new InvalidOperationException("This type cannot be converted into a lua type")
        };

        /// <param name="side">Original side</param>
        /// <returns>The opposite side</returns>
        public static Sides Opposite(this Sides side) => (int)side % 2 == 0 ? side + 1 : side - 1;

        public static string ToLiteral(this string input)
        {
            StringBuilder literal = new(input.Length + 2);
            literal.Append('"');
            foreach (var c in input)
            {
                switch (c)
                {
                    case '\"': literal.Append("\\\""); break;
                    case '\\': literal.Append(@"\\"); break;
                    case '\0': literal.Append(@"\0"); break;
                    case '\a': literal.Append(@"\a"); break;
                    case '\b': literal.Append(@"\b"); break;
                    case '\f': literal.Append(@"\f"); break;
                    case '\n': literal.Append(@"\n"); break;
                    case '\r': literal.Append(@"\r"); break;
                    case '\t': literal.Append(@"\t"); break;
                    case '\v': literal.Append(@"\v"); break;
                    default:
                        // ASCII printable character
                        if (c >= 0x20 && c <= 0x7e)
                        {
                            literal.Append(c);
                            // As UTF16 escaped character
                        }
                        else
                        {
                            literal.Append(@"\u");
                            literal.Append(((int)c).ToString("x4"));
                        }
                        break;
                }
            }
            literal.Append('"');
            return literal.ToString();
        }
    }
}
