using EasyJSON;
using RemoteOS.OpenComputers.Data;
using RemoteOS.OpenComputers.Exceptions;
using System.Collections.ObjectModel;
using System.Drawing;

namespace RemoteOS.OpenComputers.Components
{
    [Component("geolyzer")]
    public class GeolyzerComponent : Component
    {
        public GeolyzerComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <inheritdoc cref="Scan(int, int, int, int, int, int, bool)"/>
        public async Task<double[,,]> Scan(int x, int z, bool ignoreReplaceable = false) => await Scan(x, z, -32, 1, 1, 64, ignoreReplaceable);
        /// <summary>
        /// Analyzes the density of the column at the specified relative coordinates.
        /// </summary>
        /// <param name="x">X column coordinate</param>
        /// <param name="z">Z column coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="width">Width(X size) of the scan</param>
        /// <param name="depth">Depth(Z size) of the scan</param>
        /// <param name="height">Height(Y size) of the scan</param>
        /// <param name="ignoreReplaceable">Should ignore replaceable blocks</param>
        /// <exception cref="GeolyzerException">The scan boundaries are invalid</exception>
        /// <returns>Scanned blocks hardness matrix, [x,y,z]</returns>
        public async Task<double[,,]> Scan(int x, int z, int y, int width, int depth, int height, bool ignoreReplaceable = false)
        {
            if (width < 0 || depth < 0 || height < 0) throw new GeolyzerException("Invalid dimensions (Size cannot be negative)");
            if (width * depth * height > 64) throw new GeolyzerException("Volume too large (Maximum is 64)");
            var maxRange = (await this.GetDeviceInfo()).Capacity;
            var maxX = x + width - 1;
            var maxY = y + height - 1;
            var maxZ = z + depth - 1;
            if(x > maxRange || y > maxRange || z > maxRange || maxX > maxRange || maxY > maxRange || maxZ > maxRange) throw new GeolyzerException("Location out of bounds");
            var res = await Parent.Execute($@"table.unpack({await GetHandle()}.scan({x},{z},{y},{width},{depth},{height},{ignoreReplaceable.Luaify()}))");
            var raw = res.Linq.Select(x => x.Value.AsDouble).ToArray();
            var ret = new double[width, height, depth];
            var i = 0;
            for (int ry = 0; ry < height; ry++)
                for (int rz = 0; rz < depth; rz++)
                    for (int rx = 0; rx < width; rx++)
                        ret[rx, ry, rz] = raw[i++];
            return ret;
        }
        /// <summary>
        /// Get some information on a directly adjacent block.
        /// </summary>
        /// <param name="side">Which side to analyze</param>
        /// <returns>Block information</returns>
        public async Task<GeolyzerResult> Analyze(Sides side)
        {
            var res = (await Invoke("analyze", side))[0];
            return new()
            {
                Color = Color.FromArgb(res["color"]),
                Hardness = res["hardness"],
                HarvestLevel = res["harvestLevel"],
                HarvestTool = res["harvestTool"],
                Meta = res["metadata"],
                Name = res["name"],
                Properties = new(res["properties"].Linq.ToDictionary(x => x.Key, x => x.Value))
            };
        }
        /// <summary>
        /// Store an item stack representation of the block on the specified side in a database component.
        /// </summary>
        /// <param name="side">Which side to analyze</param>
        /// <param name="database">Destination database</param>
        /// <param name="dbSlot">Database slot</param>
        /// <returns>true if block was stored successfully</returns>
        public async Task<bool> Store(Sides side, DatabaseComponent database, int dbSlot) => (await Invoke("store", side, database, dbSlot))[0];
        /// <summary>
        /// Checks the contents of the block on the specified sides and returns the findings.
        /// </summary>
        /// <param name="side">Which side to analyze</param>
        /// <returns>Whether the block is passable and its description</returns>
        public async Task<(bool Passable, string Description)> Detect(Sides side)
        {
            var res = await Invoke("detect", side);
            return (res[0], res[1]);
        }
        /// <returns>Whether there is a clear line of sight to the sky directly above.</returns>
        public async Task<bool> CanSeeSky() => (await Invoke("canSeeSky"))[0];
        /// <returns>Whether the sun is currently visible directly above.</returns>
        public async Task<bool> IsSunVisible() => (await Invoke("isSunVisible"))[0];

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public bool SkyVisible => CanSeeSky().Result;
        public bool SunVisible => IsSunVisible().Result;
#endif
    }
}
