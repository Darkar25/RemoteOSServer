using RemoteOS.OpenComputers.Exceptions;
using System.Drawing;
using System.Numerics;

namespace RemoteOS.OpenComputers.Components
{
    //Palette and content is not cached since the hologram is EXTERNAL component and some other machine may modify its data
    [Component("hologram")]
    public class HologramComponent : Component
    {
        public const int Width = 48;
        public const int Height = 32;

        public HologramComponent(Machine parent, Guid address) : base(parent, address) { }

        int? _maxDepth;
        public readonly Vector3 Dimensions = new(Width, Height, Width);

        public async Task<Color> GetPaletteColor(int index)
        {
            if (index <= 0 || (await GetMaxDepth() == 1 ? index > 1 : index > 3)) throw new PaletteException("Invalid palette index");
            return Color.FromArgb((await Invoke("getPaletteColor", index))[0]);
        }
        public async Task<Color> SetPaletteColor(int index, Color color)
        {
            if (index <= 0 || (await GetMaxDepth() == 1 ? index > 1 : index > 3)) throw new PaletteException("Invalid palette index");
            return Color.FromArgb((await Invoke("setPaletteColor", index, color.ToArgb()))[0]);
        }
        public async Task<int> GetMaxDepth() => _maxDepth ??= (await Invoke("maxDepth"))[0];
        public async Task Clear() => await Invoke("clear");
        public async Task<int> Get(int x, int y, int z)
        {
            if (x < 0 || x >= Dimensions.X) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Dimensions.Y) throw new ArgumentOutOfRangeException(nameof(x));
            if (z < 0 || z >= Dimensions.Z) throw new ArgumentOutOfRangeException(nameof(z));
            return (await Invoke("get", x, y, z))[0];
        }
        public async Task<int> Get(Vector3 pos) => await Get((int)pos.X, (int)pos.Y, (int)pos.Z);
        public async Task Set(int x, int y, int z, int index)
        {
            if (x < 0 || x >= Dimensions.X) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Dimensions.Y) throw new ArgumentOutOfRangeException(nameof(x));
            if (z < 0 || z >= Dimensions.Z) throw new ArgumentOutOfRangeException(nameof(z));
            await Invoke("set", x, y, z, index);
        }
        public async Task Set(int x, int y, int z, bool ison) => await Set(x, y, z, ison ? 1 : 0);
        public async Task Set(Vector3 pos, int index) => await Set((int)pos.X, (int)pos.Y, (int)pos.Z, index);
        public async Task Set(Vector3 pos, bool ison) => await Set((int)pos.X, (int)pos.Y, (int)pos.Z, ison);
        public async Task Fill(int x, int z, int maxY, int index, int minY = 1)
        {
            if (x < 0 || x >= Dimensions.X) throw new ArgumentOutOfRangeException(nameof(x));
            if (z < 0 || z >= Dimensions.Z) throw new ArgumentOutOfRangeException(nameof(z));
            if (minY <= 0 || minY >= Dimensions.Y) throw new ArgumentOutOfRangeException(nameof(minY));
            if (maxY <= 0 || maxY >= Dimensions.Y) throw new ArgumentOutOfRangeException(nameof(maxY));
            if (minY > maxY) throw new ArgumentOutOfRangeException(nameof(minY), "Interval is empty");
            await Invoke("fill", x, z, minY, maxY, index);
        }
        public async Task Fill(int x, int z, int maxY, bool ison, int minY = 1) => await Fill(x, z, maxY, ison ? 1 : 0, minY);
        public async Task SetRaw(string data) => await Invoke("setRaw", @$"""{data}""");
        public async Task Copy(int x, int z, int width, int height, int tx, int ty)
        {
            if (x < 0 || x >= Dimensions.X) throw new ArgumentOutOfRangeException(nameof(x));
            if (z < 0 || z >= Dimensions.Z) throw new ArgumentOutOfRangeException(nameof(z));
            if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException("Size cannot be negative");
            if (tx == 0 && ty == 0) return;
            await Invoke("copy", x, z, width, height, tx, ty);
        }
        public async Task<float> GetScale() => (await Invoke("getScale"))[0];
        public async Task SetScale(float scale)
        {
            if (scale < 1f / 3f) throw new ArgumentOutOfRangeException(nameof(scale), "Scale is too small");
            await Invoke("setScale", scale);
        }
        public async Task<Vector3> GetTranslation()
        {
            var res = await Invoke("getTranslation");
            return new(res[0], res[1], res[2]);
        }
        public async Task SetTranslation(float x, float y, float z)
        {
            if (y < 0) throw new ArgumentOutOfRangeException(nameof(y), "Unsupported translation value");
            await Invoke("setTranslation", x, y, z);
        }
        public async Task SetTranslation(Vector3 pos) => await SetTranslation(pos.X, pos.Y, pos.Z);
        public async Task<bool> SetRotation(float angle, float x, float y, float z)
        {
            if (await GetMaxDepth() < 2) throw new NotSupportedException("Rotation is not supported on this tier of holographic projector");
            return (await Invoke("setRotation", angle, x, y, z))[0];
        }
        public async Task<bool> SetRotationSpeed(float speed, float x, float y, float z)
        {
            if (await GetMaxDepth() < 2) throw new NotSupportedException("Rotation is not supported on this tier of holographic projector");
            if (speed < -340 * 4 || speed > 340 * 4) throw new ArgumentOutOfRangeException(nameof(speed), "Rotation cannot be that fast");
            return (await Invoke("setRotationSpeed", speed, x, y, z))[0];
        }

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public int MaxDepth => GetMaxDepth().Result;
        public float Scale
        {
            get => GetScale().Result;
            set => SetScale(value);
        }
        public Vector3 Translation
        {
            get => GetTranslation().Result;
            set => SetTranslation(value);
        }
#endif
    }
}
