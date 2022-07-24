namespace RemoteOS.OpenComputers.Components
{
    [Component("screen")]
    public class ScreenComponent : Component
    {
        /// <summary>
        /// This event is sent when screen changes size
        /// <para>Parameters:
        /// <br>int - new width</br>
        /// <br>int - new height</br>
        /// </para>
        /// </summary>
        public event Action<int, int>? ScreenResized;
        /// <summary>
        /// This event is sent when player touched the screen
        /// <para>Parameters:
        /// <br>int - X coordinate of the touch</br>
        /// <br>int - Y coordinate of the touch</br>
        /// <br>int - mouse button number</br>
        /// <br>string - player name</br>
        /// </para>
        /// </summary>
        public event Action<int, int, int, string?>? Touch;
        /// <summary>
        /// This event is sent when player dragged the mouse over the screen
        /// <para>Parameters:
        /// <br>int - X coordinate of the touch</br>
        /// <br>int - Y coordinate of the touch</br>
        /// <br>int - mouse button number</br>
        /// <br>string - player name</br>
        /// </para>
        /// </summary>
        public event Action<int, int, int, string?>? Drag;
        /// <summary>
        /// This event is sent when player released the mouse button
        /// <para>Parameters:
        /// <br>int - X coordinate of the touch</br>
        /// <br>int - Y coordinate of the touch</br>
        /// <br>int - mouse button number</br>
        /// <br>string - player name</br>
        /// </para>
        /// </summary>
        public event Action<int, int, int, string?>? Drop;
        /// <summary>
        /// This event is sent when player scrolled the mouse wheel
        /// <para>Parameters:
        /// <br>int - X coordinate of the touch</br>
        /// <br>int - Y coordinate of the touch</br>
        /// <br>int - scroll value(positive - scroll up, negative - scroll down)</br>
        /// <br>string - player name</br>
        /// </para>
        /// </summary>
        public event Action<int, int, int, string?>? Scroll;
        /// <summary>
        /// This event is sent when player walked over the screen
        /// <para>Parameters:
        /// <br>int - X coordinate of the touch</br>
        /// <br>int - Y coordinate of the touch</br>
        /// <br>string - player name</br>
        /// </para>
        /// </summary>
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

        /// <returns>The aspect ratio of the screen. For multi-block screens this is the number of blocks, horizontal and vertical.</returns>
        public async Task<(int Width, int Height)> GetAspectRatio()
        {
            var res = await Invoke("getAspectRatio");
            return (res[0], res[1]);
        }
        /// <returns>Whether the screen is currently on.</returns>
        public async Task<bool> IsOn() => (await Invoke("isOn"))[0];
        /// <summary>
        /// Turns the screen on.
        /// </summary>
        /// <returns>true if it was off.</returns>
        public async Task<bool> TurnOn() => (await Invoke("turnOn"))[0];
        /// <summary>
        /// Turns off the screen.
        /// </summary>
        /// <returns>true if it was on.</returns>
        public async Task<bool> TurnOff() => (await Invoke("turnOff"))[0];
        /// <returns>The list of keyboards attached to the screen.</returns>
        public async Task<IEnumerable<KeyboardComponent>> GetKeyboards()
        {
            var comps = await Parent.GetComponents();
            return (await Invoke("getKeyboards")).Linq.Select(x => comps.Get<KeyboardComponent>(Guid.Parse(x.Value.Value))).Where(x => x is not null);
        }
        /// <returns>Whether the screen is in high precision mode (sub-pixel mouse event positions).</returns>
        public async Task<bool> IsPrecise() => (await Invoke("isPrecise"))[0];
        /// <param name="precise">Set whether to use high precision mode (sub-pixel mouse event positions).</param>
        /// <returns>The old value</returns>
        public async Task<bool> SetPrecise(bool precise) => (await Invoke("setPrecise", precise))[0];
        /// <returns>Whether touch mode is inverted (sneak-activate opens GUI, instead of normal activate).</returns>
        public async Task<bool> IsTouchModeInverted() => (await Invoke("isTouchModeInverted"))[0];
        /// <summary>
        /// Sets whether to invert touch mode (sneak-activate opens GUI, instead of normal activate).
        /// </summary>
        /// <param name="inverted">Touch mode</param>
        /// <returns>The old value</returns>
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

        public IEnumerable<KeyboardComponent> Keyboards => GetKeyboards().Result;

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
