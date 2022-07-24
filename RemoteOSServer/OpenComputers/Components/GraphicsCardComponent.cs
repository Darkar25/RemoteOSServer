using RemoteOS.OpenComputers.Exceptions;
using System.Drawing;

namespace RemoteOS.OpenComputers.Components
{
    //Perhaps add a buffer data caching too?
    [Component("gpu")]
    public class GraphicsCardComponent : Component
    {
        public const int RESERVED_SCREEN = 0;
        public GraphicsCardComponent(Machine parent, Guid address) : base(parent, address) { }

        List<GPUBuffer> _buffers = new();
        GPUBuffer? _screenBuffer;
        GPUBuffer? _selectedBuffer;
        int? _totalMemory;
        int? _maxWidth;
        int? _maxHeight;
        ScreenComponent? _screen;
        Color? _background;
        Color? _foreground;
        int? _depth;
        int? _maxDepth;
        int? _width;
        int? _height;
        int? _vwidth;
        int? _vheight;
        List<Color?> _palette = new();

        /// <param name="index">Palette index</param>
        /// <returns>The palette color at the specified palette index.</returns>
        /// <exception cref="PaletteException">Palette is not available or the index is invalid</exception>
        public async Task<Color> GetPaletteColor(int index)
        {
            if(await GetDepth() == 1) throw new PaletteException("Palette is not available");
            if (index < 0 || index >= 16) throw new PaletteException("Invalid palette index");
            return _palette[index] ??= Color.FromArgb((await Invoke("getPaletteColor", index))[0]);
        }
        /// <summary>
        /// Set the palette color at the specified palette index.
        /// </summary>
        /// <param name="index">Palette index</param>
        /// <param name="color">New color</param>
        /// <returns>The previous value.</returns>
        /// <exception cref="PaletteException">Palette is not available or the index is invalid</exception>
        public async Task<Color> SetPaletteColor(int index, Color color)
        {
            if (await GetDepth() == 1) throw new PaletteException("Palette is not available");
            if (index < 0 || index >= 16) throw new PaletteException("Invalid palette index");
            _palette[index] = color;
            return Color.FromArgb((await Invoke("setPaletteColor", index, color.ToArgb()))[0]);
        }
        /// <returns>The currently selected buffer</returns>
        public GPUBuffer GetSelectedBuffer() => _selectedBuffer ??= ScreenBuffer;
        /// <summary>
        /// Sets the active buffer
        /// </summary>
        /// <param name="buffer">Buffer to select</param>
        /// <exception cref="InvalidOperationException">The buffer is invalid</exception>
        public async Task SetSelectedBuffer(GPUBuffer buffer)
        {
            if (buffer.Parent != this) throw new InvalidOperationException("The buffer does not belong to this GPU");
            if (buffer.Handle != RESERVED_SCREEN && !_buffers.Contains(buffer)) throw new InvalidOperationException("Invalid buffer");
            var res = await Invoke("setActiveBuffer", buffer.Handle);
            if(res[1].IsNull)
                _selectedBuffer = buffer;
        }
        /// <returns>An array of the allocated buffers</returns>
        public IEnumerable<GPUBuffer> GetBuffers() => _buffers.AsEnumerable();
        /// <inheritdoc cref="AllocateBuffer(int, int)"/>
        public async Task<GPUBuffer> AllocateBuffer()
        {
            var max = await GetMaxResolution();
            return await AllocateBuffer(max.Width, max.Height);
        }
        /// <summary>
        /// Allocates a new buffer with dimensions width*height (defaults to max resolution) and appends it to the buffer list.
        /// </summary>
        /// <param name="width">Width of the new buffer</param>
        /// <param name="height">Height of the new buffer</param>
        /// <returns>Allocated buffer</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalig buffer dimensions</exception>
        /// <exception cref="OutOfMemoryException">Graphics card ran out of memory</exception>
        public async Task<GPUBuffer> AllocateBuffer(int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), "Invalid page dimensions: must be greater than zero");
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), "Invalid page dimensions: must be greater than zero");
            if (width * height > await GetFreeMemory()) throw new OutOfMemoryException("Not enough video memory");
            var id = (await Invoke("allocateBuffer", width, height))[0];
            var buf = new GPUBuffer(this, id, width, height);
            _buffers.Add(buf);
            return buf;
        }
        /// <summary>
        /// Closes all buffers and returns the count. If the active buffer is closed, selected buffer moves to screen buffer
        /// </summary>
        /// <returns>The amount of closed buffers</returns>
        public async Task<int> FreeAllBuffers()
        {
            _buffers.Clear();
            _selectedBuffer = ScreenBuffer;
            return (await Invoke("freeAllBuffers"))[0];
        }
        /// <returns>The total memory size of the gpu vram. This does not include the screen.</returns>
        public async Task<int> GetTotalMemory() => _totalMemory ??= (await Invoke("totalMemory"))[0];
        /// <returns>The total free memory not allocated to buffers. This does not include the screen.</returns>
        public async Task<int> GetFreeMemory() => await GetTotalMemory() - GetBuffers().Sum(x => x.Width * x.Height);
        /// <returns>The maximum screen resolution.</returns>
        public async Task<(int Width, int Height)> GetMaxResolution()
        {
            if (_maxWidth == null || _maxHeight == null)
            {
                var res = await Invoke("maxResolution");
                _maxWidth = res[0];
                _maxHeight = res[1];
            }
            return (_maxWidth.Value, _maxHeight.Value);
        }
        /// <summary>
        /// Binds the GPU to the screen with the specified address and resets screen settings if `reset` is true.
        /// </summary>
        /// <param name="screen">The screen to bind to</param>
        /// <param name="reset">Reset the screen settings</param>
        /// <returns>true if binding was successful</returns>
        public async Task<bool> Bind(ScreenComponent screen, bool reset = true)
        {
            bool ret = (await Invoke("bind", $@"""{screen.Address}""", reset))[0];
            if(ret) {
                _maxWidth = null;
                _maxHeight = null;
                _maxDepth = null;
                if(reset)
                {
                    _background = Color.Black;
                    _foreground = Color.White;
                    _depth = await GetMaxDepth();
                    (_width, _height) = await GetMaxResolution();
                    _screenBuffer = new GPUBuffer(this, RESERVED_SCREEN, _maxWidth.Value, _maxHeight.Value);
                    if (GetSelectedBuffer().Handle == RESERVED_SCREEN) _selectedBuffer = _screenBuffer;
                }
            }
            return ret;
        }
        /// <returns>The screen the GPU is currently bound to.</returns>
        public async Task<ScreenComponent> GetScreen() => _screen ??= (await Parent.GetComponents()).Get<ScreenComponent>(Guid.Parse((await Invoke("getScreen"))[0]));
        /// <returns>The current background color and whether it's from the palette or not.</returns>
        public async Task<(Color color, int PaletteIndex)> GetBackground()
        {
            if (_background == null) {
                var res = await Invoke("getBackground");
                if (res[1])
                    return ((_background = await GetPaletteColor(res[0])).Value, res[0]);
                return((_background = Color.FromArgb(res[0])).Value, -1);
            }
            return (_background.Value, _palette.IndexOf(_background));
        }
        /// <summary>
        /// Sets the background color to the specified value. Optionally takes an explicit palette index.
        /// </summary>
        /// <param name="background">New background color</param>
        /// <returns>The old value and if it was from the palette its palette index.</returns>
        public async Task<(Color color, int PaletteIndex)> SetBackground(Color background)
        {
            _background = background;
            var res = await Invoke("setBackground", background.ToArgb());
            return (Color.FromArgb(res[0]), res[1].IsNull ? -1 : res[1]);
        }
        /// <inheritdoc cref="SetBackground(Color)"/>
        /// <param name="paletteIndex">Palette index</param>
        public async Task<(Color color, int PaletteIndex)> SetBackground(int paletteIndex)
        {
            _background = await GetPaletteColor(paletteIndex);
            var res = await Invoke("setBackground", paletteIndex, true);
            return (Color.FromArgb(res[0]), res[1].IsNull ? -1 : res[1]);
        }
        /// <returns>The current foreground color and whether it's from the palette or not.</returns>
        public async Task<(Color color, int PaletteIndex)> GetForeground()
        {
            if (_foreground == null)
            {
                var res = await Invoke("getForeground");
                if (res[1])
                    return ((_foreground = await GetPaletteColor(res[0])).Value, res[0]);
                return ((_foreground = Color.FromArgb(res[0])).Value, -1);
            }
            return (_foreground.Value, _palette.IndexOf(_foreground));
        }
        /// <summary>
        /// Sets the foreground color to the specified value. Optionally takes an explicit palette index.
        /// </summary>
        /// <param name="foreground">New foreground color</param>
        /// <returns>The old value and if it was from the palette its palette index.</returns>
        public async Task<(Color color, int PaletteIndex)> SetForeground(Color foreground)
        {
            _foreground = foreground;
            var res = await Invoke("setForeground", foreground.ToArgb());
            return (Color.FromArgb(res[0]), res[1].IsNull ? -1 : res[1]);
        }
        /// <inheritdoc cref="SetForeground(Color)"/>
        /// <param name="paletteIndex">Palette index</param>
        public async Task<(Color color, int PaletteIndex)> SetForeground(int paletteIndex)
        {
            _foreground = await GetPaletteColor(paletteIndex);
            var res = await Invoke("setForeground", paletteIndex, true);
            return (Color.FromArgb(res[0]), res[1].IsNull ? -1 : res[1]);
        }
        /// <returns>The currently set color depth.</returns>
        public async Task<int> GetDepth() => _depth ??= (await Invoke("getDepth"))[0];
        /// <summary>
        /// Set the color depth.
        /// </summary>
        /// <param name="depth">New depth value</param>
        /// <returns>The previous value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">This depth is not supported</exception>
        public async Task<int> SetDepth(int depth)
        {
            if (depth != 1 || depth != 4 || depth != 8 || depth > await GetMaxDepth()) throw new ArgumentOutOfRangeException("depth", "Unsupported depth");
            _depth = depth;
            return (await Invoke("setDepth", depth))[0];
        }
        /// <returns>The maximum supported color depth.</returns>
        public async Task<int> GetMaxDepth() => _maxDepth ??= (await Invoke("maxDepth"))[0];
        /// <returns>The current screen resolution.</returns>
        public async Task<(int Width, int Height)> GetResolution()
        {
            if (_width == null || _height == null)
            {
                var res = await Invoke("getResolution");
                _width = res[0];
                _height = res[1];
            }
            return (_width.Value, _height.Value);
        }
        /// <summary>
        /// Set the screen resolution.
        /// </summary>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        /// <returns>true if the resolution changed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">This resolution is not supported</exception>
        public async Task<bool> SetResolution(int width, int height)
        {
            var max = await GetMaxResolution();
            if (width < 1 || width > max.Width) throw new ArgumentOutOfRangeException(nameof(width), "Unsupported resolution");
            if (height < 1 || height > max.Height) throw new ArgumentOutOfRangeException(nameof(height), "Unsupported resolution");
            if (width * height > max.Width * max.Height) throw new ArgumentOutOfRangeException("Unsupported resolution");
            var ret = (await Invoke("setResolution", width, height))[0];
            if (ret)
            {
                _width = width;
                _height = height;
            }
            return ret;
        }
        /// <returns>The current viewport resolution.</returns>
        public async Task<(int Width, int Height)> GetViewport()
        {
            if (_vwidth == null || _vheight == null)
            {
                var res = await Invoke("getViewport");
                _vwidth = res[0];
                _vheight = res[1];
            }
            return (_vwidth.Value, _vheight.Value);
        }
        /// <summary>
        /// Set the viewport resolution. Cannot exceed the screen resolution.
        /// </summary>
        /// <param name="width">New viewport width</param>
        /// <param name="height">New viewport height</param>
        /// <returns>true if the resolution changed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">This resolution is not supported</exception>
        public async Task<bool> SetViewport(int width, int height)
        {
            var max = await GetMaxResolution();
            if (width < 1 || width > max.Width) throw new ArgumentOutOfRangeException(nameof(width), "Unsupported resolution");
            if (height < 1 || height > max.Height) throw new ArgumentOutOfRangeException(nameof(height), "Unsupported resolution");
            if (width * height > max.Width * max.Height) throw new ArgumentOutOfRangeException("Unsupported resolution");
            var ret = (await Invoke("setViewport", width, height))[0];
            if (ret)
            {
                _vwidth = width;
                _vheight = height;
            }
            return ret;
        }
        /// <param name="x">Pixel X coordinate</param>
        /// <param name="y">Pixel Y coordinate</param>
        /// <returns>The value displayed on the screen at the specified index, as well as the foreground and background color. If the foreground or background is from the palette, returns the palette indices</returns>
        public async Task<(string Value, Color Foreground, Color Background, int? ForegroundIndex, int? BackgroundIndex)> Get(int x,int y)
        {
            var ret = await Invoke("get", x, y);
            return (ret[0], Color.FromArgb(ret[1]), Color.FromArgb(ret[2]), ret[3], ret[4]);
        }
        /// <summary>
        /// Plots a string value to the screen at the specified position. Optionally writes the string vertically.
        /// </summary>
        /// <param name="x">Column to write to</param>
        /// <param name="y">Row to write to</param>
        /// <param name="value">What to print</param>
        /// <param name="vertical">Print the text vertically</param>
        /// <returns>true if something was printed</returns>
        public async Task<bool> Set(int x, int y, string value, bool vertical = false) => (await Invoke("set", x, y, $@"""{value}""", vertical))[0];
        /// <summary>
        /// Copies a portion of the screen from the specified location with the specified size by the specified translation.
        /// </summary>
        /// <param name="x">Column to copy from</param>
        /// <param name="y">Row to copy from</param>
        /// <param name="width">Width of the region</param>
        /// <param name="height">Heoght of the region</param>
        /// <param name="tx">Translation on the X axis</param>
        /// <param name="ty">Translation on the Y axis</param>
        /// <returns>true if copied successfully</returns>
        /// <exception cref="ArgumentOutOfRangeException">The size of the copied region cannot be negative</exception>
        public async Task<bool> Copy(int x, int y, int width, int height, int tx, int ty)
        {
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width), "Size cannot be negative");
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height), "Size cannot be negative");
            if (tx == 0 && ty == 0) return true;
            return (await Invoke("copy", x, y, width, height, tx, ty))[0];
        }
        /// <summary>
        /// Fills a portion of the screen at the specified position with the specified size with the specified character.
        /// </summary>
        /// <param name="x">Column to start filling from</param>
        /// <param name="y">Row to start filling from</param>
        /// <param name="width">Width of filled region</param>
        /// <param name="height">Height of filled region</param>
        /// <param name="value">The character to fill the region with</param>
        /// <returns>true if region was filled</returns>
        /// <exception cref="ArgumentOutOfRangeException">Size of the region cannot be negative</exception>
        public async Task<bool> Fill(int x, int y, int width, int height, char value)
        {
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width), "Size cannot be negative");
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height), "Size cannot be negative");
            return (await Invoke("fill", x, y, width, height, $@"""{value}"""))[0];
        }
        public GPUBuffer ScreenBuffer
        {
            get
            {
                if(_screenBuffer == null)
                {
                    var max = GetMaxResolution().Result;
                    _screenBuffer = new GPUBuffer(this, RESERVED_SCREEN, max.Width, max.Height);
                }
                return _screenBuffer;
            }
        }

#if ROS_PROPERTIES
        public int Depth
        {
            get => GetDepth().Result;
            set => SetDepth(value);
        }
        public int MaxDepth => GetMaxDepth().Result;
        public GPUBuffer SelectedBuffer
        {
            get => GetSelectedBuffer();
            set => SetSelectedBuffer(value);
        }
        public Color Background => GetBackground().Result.color;
        public Color Foreground => GetForeground().Result.color;
#endif

        public class GPUBuffer : IDisposable
        {
            public int Handle { get; }
            public GraphicsCardComponent Parent { get; }
            /// <summary>
            /// The width of a buffer
            /// </summary>
            public int Width { get; }
            /// <summary>
            /// The height of a buffer
            /// </summary>
            public int Height { get; }

            public GPUBuffer(GraphicsCardComponent parent, int handle, int width, int height)
            {
                Parent = parent;
                Handle = handle;
                Width = width;
                Height = height;
            }

            /// <inheritdoc cref="Bitblt(GPUBuffer, int, int, int, int, int, int)"/>
            public async Task<bool> Bitblt(int column = 1, int row = 1, int fromColumn = 1, int fromRow = 1) => await Bitblt(Parent._buffers.First(), column, row, fromColumn, fromRow);
            /// <inheritdoc cref="Bitblt(GPUBuffer, int, int, int, int, int, int)"/>
            public async Task<bool> Bitblt(int width, int height, int column = 1, int row = 1, int fromColumn = 1, int fromRow = 1) => await Bitblt(Parent._buffers.First(), width, height, column, row, fromColumn, fromRow);
            /// <inheritdoc cref="Bitblt(GPUBuffer, int, int, int, int, int, int)"/>
            public async Task<bool> Bitblt(GPUBuffer destination, int column = 1, int row = 1, int fromColumn = 1, int fromRow = 1) => await Bitblt(destination, destination.Width, destination.Height, column, row, fromColumn, fromRow);
            /// <summary>
            /// bitblt from buffer to screen. All parameters are optional. Writes to `destination` page in rectangle `column, row, width, height`, defaults to the bound screen and its viewport. Reads data from this buffer's page at `fromColumn, fromRow`, default is the active page from position 1, 1
            /// </summary>
            /// <param name="destination">Destination buffer</param>
            /// <param name="width">Width of copied region</param>
            /// <param name="height">Height of copied region</param>
            /// <param name="column">The column to copy to</param>
            /// <param name="row">The row to copy to</param>
            /// <param name="fromColumn">The column to copy from</param>
            /// <param name="fromRow">The row to copy from</param>
            /// <returns>true if copy was successful</returns>
            /// <exception cref="InvalidOperationException">This buffer is invalid</exception>
            public async Task<bool> Bitblt(GPUBuffer destination, int width, int height, int column = 1, int row = 1, int fromColumn = 1, int fromRow = 1)
            {
                if(destination.Parent != Parent) throw new InvalidOperationException("The buffer does not belong to this GPU");
                if (!Parent._buffers.Contains(this)) throw new InvalidOperationException("Invalid buffer");
                return (await Parent.Invoke("bitblt", destination.Handle, column, row, width, height, Handle, fromColumn, fromRow))[0];
            }
            /// <summary>
            /// Closes this buffer. If the current buffer is closed, selected buffer moves to screen buffer
            /// </summary>
            /// <returns></returns>
            public async Task<bool> Free()
            {
                if (Handle == RESERVED_SCREEN) return false; //Can`t free reserved buffer
                if (Handle != RESERVED_SCREEN && !Parent._buffers.Contains(this)) return false; //Invalid buffer
                bool ret = (await Parent.Invoke("freeBuffer", Handle))[0];
                if (ret)
                {
                    Parent._buffers.Remove(this);
                    if (Parent._selectedBuffer == this) Parent._selectedBuffer = Parent.ScreenBuffer;
                }
                return ret;
            }

            public void Dispose()
            {
                Free();
            }
        }
    }
}
