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

        /// <param name="index">Palette index</param>
        /// <returns>The color defined for the specified value.</returns>
        /// <exception cref="PaletteException">This palette index is invalid</exception>
        public async Task<Color> GetPaletteColor(int index)
        {
            if (index <= 0 || (await GetMaxDepth() == 1 ? index > 1 : index > 3)) throw new PaletteException("Invalid palette index");
            return Color.FromArgb((await Invoke("getPaletteColor", index))[0]);
        }
        /// <summary>
        /// Set the color defined for the specified value.
        /// </summary>
        /// <param name="index">Palette index</param>
        /// <param name="color">New color</param>
        /// <returns>The old color</returns>
        /// <exception cref="PaletteException">This palette index is invalid</exception>
        public async Task<Color> SetPaletteColor(int index, Color color)
        {
            if (index <= 0 || (await GetMaxDepth() == 1 ? index > 1 : index > 3)) throw new PaletteException("Invalid palette index");
            return Color.FromArgb((await Invoke("setPaletteColor", index, color.ToArgb()))[0]);
        }
        public async Task<int> GetMaxDepth() => _maxDepth ??= (await Invoke("maxDepth"))[0];
        /// <summary>
        /// Clears the hologram.
        /// </summary>
        public async Task Clear() => await Invoke("clear");
        /// <param name="x">X coordinate of the voxel</param>
        /// <param name="y">Y coordinate of the voxel</param>
        /// <param name="z">Z coordinate of the voxel</param>
        /// <returns>The value for the specified voxel.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The coordinates are outside of the hologram bounds</exception>
        public async Task<int> Get(int x, int y, int z)
        {
            if (x < 0 || x >= Dimensions.X) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Dimensions.Y) throw new ArgumentOutOfRangeException(nameof(x));
            if (z < 0 || z >= Dimensions.Z) throw new ArgumentOutOfRangeException(nameof(z));
            return (await Invoke("get", x, y, z))[0];
        }
        /// <inheritdoc cref="Get(int, int, int)"/>
        /// <param name="pos">Position of the voxel</param>
        public async Task<int> Get(Vector3 pos) => await Get((int)pos.X, (int)pos.Y, (int)pos.Z);
        /// <summary>
        /// Set the value for the specified voxel.
        /// </summary>
        /// <param name="x">X coordinate of the voxel</param>
        /// <param name="y">Y coordinate of the voxel</param>
        /// <param name="z">Z coordinate of the voxel</param>
        /// <param name="index">Palette index of the voxel</param>
        /// <exception cref="ArgumentOutOfRangeException">The coordinates are outside of the hologram bounds</exception>
        public async Task Set(int x, int y, int z, int index)
        {
            if (x < 0 || x >= Dimensions.X) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Dimensions.Y) throw new ArgumentOutOfRangeException(nameof(x));
            if (z < 0 || z >= Dimensions.Z) throw new ArgumentOutOfRangeException(nameof(z));
            await Invoke("set", x, y, z, index);
        }
        /// <inheritdoc cref="Set(int, int, int, int)"/>
        /// <param name="ison">The state of the voxel</param>
        public async Task Set(int x, int y, int z, bool ison) => await Set(x, y, z, ison ? 1 : 0);
        /// <inheritdoc cref="Set(int, int, int, int)"/>
        /// <param name="pos">Position of the voxel</param>
        public async Task Set(Vector3 pos, int index) => await Set((int)pos.X, (int)pos.Y, (int)pos.Z, index);
        /// <inheritdoc cref="Set(int, int, int, bool)"/>
        /// <param name="pos">Position of the voxel</param>
        public async Task Set(Vector3 pos, bool ison) => await Set((int)pos.X, (int)pos.Y, (int)pos.Z, ison);
        /// <summary>
        /// Fills an interval of a column with the specified value.
        /// </summary>
        /// <param name="x">Column X position</param>
        /// <param name="z">Column Z position</param>
        /// <param name="maxY">Maximum Y position</param>
        /// <param name="index">Palette index</param>
        /// <param name="minY">Minimum Y Position</param>
        /// <exception cref="ArgumentOutOfRangeException">The coordinates are outside of the hologram bounds</exception>
        public async Task Fill(int x, int z, int maxY, int index, int minY = 1)
        {
            if (x < 0 || x >= Dimensions.X) throw new ArgumentOutOfRangeException(nameof(x));
            if (z < 0 || z >= Dimensions.Z) throw new ArgumentOutOfRangeException(nameof(z));
            if (minY <= 0 || minY >= Dimensions.Y) throw new ArgumentOutOfRangeException(nameof(minY));
            if (maxY <= 0 || maxY >= Dimensions.Y) throw new ArgumentOutOfRangeException(nameof(maxY));
            if (minY > maxY) throw new ArgumentOutOfRangeException(nameof(minY), "Interval is empty");
            await Invoke("fill", x, z, minY, maxY, index);
        }
        /// <inheritdoc cref="Fill(int, int, int, int, int)"/>
        /// <param name="ison">The state of the voxel</param>
        public async Task Fill(int x, int z, int maxY, bool ison, int minY = 1) => await Fill(x, z, maxY, ison ? 1 : 0, minY);
        /// <summary>
        /// Set the raw buffer to the specified byte array, where each byte represents a voxel color. Nesting is x,z,y.
        /// </summary>
        /// <param name="data">The voxel data</param>
        public async Task SetRaw(string data) => await Invoke("setRaw", @$"""{data}""");
        /// <summary>
        /// Copies an area of columns by the specified translation.
        /// </summary>
        /// <param name="x">Column to copy from</param>
        /// <param name="z">Column to copy from</param>
        /// <param name="width">Width of the region</param>
        /// <param name="height">Heoght of the region</param>
        /// <param name="tx">X translation</param>
        /// <param name="tz">Z translation</param>
        /// <exception cref="ArgumentOutOfRangeException">The coordinates are outside of the hologram bounds</exception>
        public async Task Copy(int x, int z, int width, int height, int tx, int tz)
        {
            if (x < 0 || x >= Dimensions.X) throw new ArgumentOutOfRangeException(nameof(x));
            if (z < 0 || z >= Dimensions.Z) throw new ArgumentOutOfRangeException(nameof(z));
            if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException("Size cannot be negative");
            if (tx == 0 && tz == 0) return;
            await Invoke("copy", x, z, width, height, tx, tz);
        }
        /// <returns>The render scale of the hologram.</returns>
        public async Task<float> GetScale() => (await Invoke("getScale"))[0];
        /// <summary>
        /// Set the render scale. A larger scale consumes more energy.
        /// </summary>
        /// <param name="scale">The new scale</param>
        /// <exception cref="ArgumentOutOfRangeException">The scale is too small</exception>
        public async Task SetScale(float scale)
        {
            if (scale < 1f / 3f) throw new ArgumentOutOfRangeException(nameof(scale), "Scale is too small");
            await Invoke("setScale", scale);
        }
        /// <returns>The relative render projection offsets of the hologram.</returns>
        public async Task<Vector3> GetTranslation()
        {
            var res = await Invoke("getTranslation");
            return new(res[0], res[1], res[2]);
        }
        /// <summary>
        /// Sets the relative render projection offsets of the hologram.
        /// </summary>
        /// <param name="x">X translation</param>
        /// <param name="y">Y translation</param>
        /// <param name="z">Z translation</param>
        /// <exception cref="ArgumentOutOfRangeException">This translation is not supported</exception>
        public async Task SetTranslation(float x, float y, float z)
        {
            if (y < 0) throw new ArgumentOutOfRangeException(nameof(y), "Unsupported translation value");
            await Invoke("setTranslation", x, y, z);
        }
        /// <inheritdoc cref="SetTranslation(float, float, float)"/>
        /// <param name="pos">Translation</param>
        public async Task SetTranslation(Vector3 pos) => await SetTranslation(pos.X, pos.Y, pos.Z);
        /// <summary>
        /// Set the base rotation of the displayed hologram.
        /// </summary>
        /// <param name="angle">Rotation angle</param>
        /// <param name="x">Rotation vector X component</param>
        /// <param name="y">Rotation vector Y component</param>
        /// <param name="z">Rotation vector Z component</param>
        /// <returns>true if rotation was successful</returns>
        /// <exception cref="NotSupportedException">This holographic projector does not support rotation</exception>
        public async Task<bool> SetRotation(float angle, float x, float y, float z)
        {
            if (await GetMaxDepth() < 2) throw new NotSupportedException("Rotation is not supported on this tier of holographic projector");
            return (await Invoke("setRotation", angle, x, y, z))[0];
        }
        /// <inheritdoc cref="SetRotation(float, float, float, float)"/>
        /// <param name="rotationVector">Rotation vector</param>
        public async Task<bool> SetRotation(float angle, Vector3 rotationVector) => await SetRotation(angle, rotationVector.X, rotationVector.Y, rotationVector.Z);
        /// <summary>
        /// Set the rotation speed of the displayed hologram.
        /// </summary>
        /// <param name="speed">Speed of the rotation</param>
        /// <param name="x">Rotation vector X component</param>
        /// <param name="y">Rotation vector Y component</param>
        /// <param name="z">Rotation vector Z component</param>
        /// <returns>true if rotation speed was set successfully</returns>
        /// <exception cref="NotSupportedException">This holographic projector does not support rotation</exception>
        /// <exception cref="ArgumentOutOfRangeException">Rotation is too fast</exception>
        public async Task<bool> SetRotationSpeed(float speed, float x, float y, float z)
        {
            if (await GetMaxDepth() < 2) throw new NotSupportedException("Rotation is not supported on this tier of holographic projector");
            if (speed < -340 * 4 || speed > 340 * 4) throw new ArgumentOutOfRangeException(nameof(speed), "Rotation cannot be that fast");
            return (await Invoke("setRotationSpeed", speed, x, y, z))[0];
        }
        /// <inheritdoc cref="SetRotationSpeed(float, float, float, float)"/>
        /// <param name="rotationVector">Rotation vector</param>
        public async Task<bool> SetRotationSpeed(float speed, Vector3 rotationVector) => await SetRotationSpeed(speed, rotationVector.X, rotationVector.Y, rotationVector.Z);

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
