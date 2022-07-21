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

        public async Task<double[,,]> Scan(int x, int z, bool ignoreReplacable = false) => await Scan(x, z, -32, 1, 1, 64, ignoreReplacable);

        public async Task<double[,,]> Scan(int x, int z, int y, int width, int depth, int height, bool ignoreReplacable = false)
        {
            if (width < 0 || depth < 0 || height < 0) throw new GeolyzerException("Invalid dimensions (Size cannot be negative)");
            if (width * depth * height > 64) throw new GeolyzerException("Volume too large (Maximum is 64)");
            var res = await Parent.Execute($@"table.unpack({Handle}.scan({x},{z},{y},{width},{depth},{height},{ignoreReplacable}))");
            var raw = res.Linq.Select(x => x.Value.AsDouble).ToArray();
            var ret = new double[width,height,depth];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    for (int k = 0; k < depth; k++)
                        ret[i, j, depth - k - 1] = raw[k + (i * width) + (j * depth * width)];
            return ret;
        }

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

        public async Task<bool> Store(Sides side, DatabaseComponent database, int dbSlot) => (await Invoke("store", side, database.Address, dbSlot))[0];

        public async Task<(bool Passable, string Description)> Detect(Sides side)
        {
            var res = await Invoke("detect", side);
            return (res[0], res[1]);
        }
        public async Task<bool> CanSeeSky() => (await Invoke("canSeeSky"))[0];
        public async Task<bool> IssSunVisible() => (await Invoke("isSunVisible"))[0];

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public bool SkyVisible => CanSeeSky().Result;
        public bool SunVisible => IssSunVisible().Result;
#endif
    }
}
