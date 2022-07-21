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

        public async Task<Color> GetPaletteColor(int index)
        {
            if(await GetDepth() == 1) throw new PaletteException("Palette is not available");
            if (index < 0 || index >= 16) throw new PaletteException("Invalid palette index");
            return _palette[index] ??= Color.FromArgb((await Invoke("getPaletteColor", index))[0]);
        }

        public async Task<Color> SetPaletteColor(int index, Color color)
        {
            if (await GetDepth() == 1) throw new PaletteException("Palette is not available");
            if (index < 0 || index >= 16) throw new PaletteException("Invalid palette index");
            _palette[index] = color;
            return Color.FromArgb((await Invoke("setPaletteColor", index, color.ToArgb()))[0]);
        }

        public GPUBuffer GetSelectedBuffer() => _selectedBuffer ??= ScreenBuffer;

        public async Task SetSelectedBuffer(GPUBuffer buffer)
        {
            if (buffer.Parent != this) throw new InvalidOperationException("The buffer does not belong to this GPU");
            if (buffer.Handle != RESERVED_SCREEN && !_buffers.Contains(buffer)) throw new InvalidOperationException("Invalid buffer");
            int id = (await Invoke("setActiveBuffer", buffer.Handle))[0];
            var buf = _buffers.FirstOrDefault(x => x.Handle == id);
            if(buf is null || buf.Handle != RESERVED_SCREEN) throw new InvalidOperationException("Something went wrong, this buffer was not allocated.");
            _selectedBuffer = buf;
        }

        public IEnumerable<GPUBuffer> GetBuffers() => _buffers.AsEnumerable();

        public async Task<GPUBuffer> AllocateBuffer()
        {
            var max = await GetMaxResolution();
            return await AllocateBuffer(max.Width, max.Height);
        }

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

        public async Task<int> FreeAllBuffers()
        {
            _buffers.Clear();
            return (await Invoke("freeAllBuffers"))[0];
        }

        public async Task<int> GetTotalMemory() => _totalMemory ??= (await Invoke("totalMemory"))[0];
        public async Task<int> GetFreeMemory() => await GetTotalMemory() - GetBuffers().Sum(x => x.Width * x.Height);
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
        public ScreenComponent? GetScreen() => _screen;
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
        public async Task<(Color color, int PaletteIndex)> SetBackground(Color background)
        {
            _background = background;
            var res = await Invoke("setBackground", background.ToArgb());
            return (Color.FromArgb(res[0]), res[1].IsNull ? -1 : res[1]);
        }
        public async Task<(Color color, int PaletteIndex)> SetBackground(int paletteIndex)
        {
            _background = await GetPaletteColor(paletteIndex);
            var res = await Invoke("setBackground", paletteIndex, true);
            return (Color.FromArgb(res[0]), res[1].IsNull ? -1 : res[1]);
        }
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
        public async Task<(Color color, int PaletteIndex)> SetForeground(Color foreground)
        {
            _foreground = foreground;
            var res = await Invoke("setForeground", foreground.ToArgb());
            return (Color.FromArgb(res[0]), res[1].IsNull ? -1 : res[1]);
        }
        public async Task<(Color color, int PaletteIndex)> SetForeground(int paletteIndex)
        {
            _foreground = await GetPaletteColor(paletteIndex);
            var res = await Invoke("setForeground", paletteIndex, true);
            return (Color.FromArgb(res[0]), res[1].IsNull ? -1 : res[1]);
        }
        public async Task<int> GetDepth() => _depth ??= (await Invoke("getDepth"))[0];
        public async Task<int> SetDepth(int depth)
        {
            if (depth != 1 || depth != 4 || depth != 8 || depth > await GetMaxDepth()) throw new ArgumentOutOfRangeException("depth", "Unsupported depth");
            _depth = depth;
            return (await Invoke("setDepth", depth))[0];
        }
        public async Task<int> GetMaxDepth() => _maxDepth ??= (await Invoke("maxDepth"))[0];
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
        public async Task<(string Value, Color Foreground, Color Background, int? ForegroundIndex, int? BackgroundIndex)> Get(int x,int y)
        {
            var ret = (await Invoke("get", x, y));
            return (ret[0], Color.FromArgb(ret[1]), Color.FromArgb(ret[2]), ret[3], ret[4]);
        }
        public async Task<bool> Set(int x, int y, string value, bool vertical = false) => (await Invoke("set", x, y, $@"""{value}""", vertical))[0];
        public async Task<bool> Copy(int x, int y, int width, int height, int tx, int ty)
        {
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width), "Size cannot be negative");
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height), "Size cannot be negative");
            return (await Invoke("copy", x, y, width, height, tx, ty))[0];
        }
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
            public int Width { get; }
            public int Height { get; }

            public GPUBuffer(GraphicsCardComponent parent, int handle, int width, int height)
            {
                Parent = parent;
                Handle = handle;
                Width = width;
                Height = height;
            }

            public async Task<bool> Bitblt(int column = 1, int row = 1, int fromColumn = 1, int fromRow = 1) => await Bitblt(Parent._buffers.First(), column, row, fromColumn, fromRow);
            public async Task<bool> Bitblt(int width, int height, int column = 1, int row = 1, int fromColumn = 1, int fromRow = 1) => await Bitblt(Parent._buffers.First(), width, height, column, row, fromColumn, fromRow);
            public async Task<bool> Bitblt(GPUBuffer destination, int column = 1, int row = 1, int fromColumn = 1, int fromRow = 1) => await Bitblt(destination, destination.Width, destination.Height, column, row, fromColumn, fromRow);
            public async Task<bool> Bitblt(GPUBuffer destination, int width, int height, int column = 1, int row = 1, int fromColumn = 1, int fromRow = 1)
            {
                if(destination.Parent != Parent) throw new InvalidOperationException("The buffer does not belong to this GPU");
                if (!Parent._buffers.Contains(this)) throw new InvalidOperationException("Invalid buffer");
                return (await Parent.Invoke("bitblt", destination.Handle, column, row, width, height, Handle, fromColumn, fromRow))[0];
            }

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
