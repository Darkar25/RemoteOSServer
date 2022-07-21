namespace RemoteOS.OpenComputers.Components
{
    [Component("screen")]
    public class ScreenComponent : Component
    {
        public event Action<int, int>? ScreenResized;
        public event Action<int, int, int, string?>? Touch;
        public event Action<int, int, int, string?>? Drag;
        public event Action<int, int, int, string?>? Drop;
        public event Action<int, int, int, string?>? Scroll;
        public event Action<int, int, string?>? Walk;
        public ScreenComponent(Machine parent, Guid address) : base(parent, address)
        {
            parent.Listen("screen_resized", (parameters) => {
                if (parameters[0].Value == Address.ToString())
                    ScreenResized?.Invoke(parameters[1], parameters[2]);
            });
            parent.Listen("touch", (parameters) => {
                if (parameters[0].Value == Address.ToString())
                    Touch?.Invoke(parameters[1], parameters[2], parameters[3], parameters[4]);
            });
            parent.Listen("drag", (parameters) => {
                if (parameters[0].Value == Address.ToString())
                    Drag?.Invoke(parameters[1], parameters[2], parameters[3], parameters[4]);
            });
            parent.Listen("drop", (parameters) => {
                if (parameters[0].Value == Address.ToString())
                    Drop?.Invoke(parameters[1], parameters[2], parameters[3], parameters[4]);
            });
            parent.Listen("scroll", (parameters) => {
                if (parameters[0].Value == Address.ToString())
                    Scroll?.Invoke(parameters[1], parameters[2], parameters[3], parameters[4]);
            });
            parent.Listen("walk", (parameters) => {
                if (parameters[0].Value == Address.ToString())
                    Walk?.Invoke(parameters[1], parameters[2], parameters[3]);
            });
        }

        public async Task<(int Width, int Height)> GetAspectRatio()
        {
            var res = await Invoke("getAspectRatio");
            return (res[0], res[1]);
        }

        public async Task<bool> IsOn() => (await Invoke("isOn"))[0];
        public async Task<bool> TurnOn() => (await Invoke("turnOn"))[0];
        public async Task<bool> TurnOff() => (await Invoke("turnOff"))[0];
        public async Task<IEnumerable<Guid>> GetKeyboards() => (await Invoke("getKeyboards")).Linq.Select(x => Guid.Parse(x.Value.Value));
        public async Task<bool> IsPrecise() => (await Invoke("isPrecise"))[0];
        public async Task<bool> SetPrecise(bool precise) => (await Invoke("setPrecise", precise))[0];
        public async Task<bool> IsTouchModeInverted() => (await Invoke("isTouchModeInverted"))[0];
        public async Task<bool> SetTouchModeInverted(bool inverted) => (await Invoke("setTouchModeInverted", inverted))[0];

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public bool IsScreenOn
        {
            get => IsOn().Result;
            set
            {
                if (value)
                    TurnOn();
                else
                    TurnOff();
            }
        }

        public IEnumerable<Guid> Keyboards => GetKeyboards().Result;

        public bool Precise
        {
            get => IsPrecise().Result;
            set => SetPrecise(value);
        }

        public bool TouchModeInverted
        {
            get => IsTouchModeInverted().Result;
            set => SetTouchModeInverted(value);
        }
#endif
    }
}
