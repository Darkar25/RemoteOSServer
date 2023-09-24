using OneOf.Types;
using RemoteOS.Helpers;
using System.Drawing;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("colors")]
    public class ColorfulComponent : Component
    {
        private Color? _color;

        public ColorfulComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        public async Task<ReasonOr<Success>> SetColor(Color color)
        {
            var res = await GetInvoker()(color);
            if (!res[0]) return res[1].Value;
            _color = color;
            return new Success();
        }

        public async Task<Color> GetColor() => _color ??= Color.FromArgb((await GetInvoker()())[0]);

        public async Task<ReasonOr<Success>> ReetColor()
        {
            var res = await GetInvoker()();
            if (!res[0]) return res[1].Value;
            _color = Color.FromArgb(-1);
            return new Success();
        }

#if ROS_PROPERTIES
        public Color Color
        {
            get => GetColor().Result;
            set => SetColor(value);
        }
#endif
    }
}