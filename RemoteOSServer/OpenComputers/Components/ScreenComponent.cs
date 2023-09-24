using RemoteOS.OpenComputers.Data;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("screen")]
    public partial class ScreenComponent : Component
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

        public override async Task<Tier> GetTier()
        {
            var d = (await this.GetDeviceInfo()).Width;
            return d == 1 ? Tier.One : d == 4 ? Tier.Two : Tier.Three; // Hardcoded maximum depth
        }

        /// <returns>The aspect ratio of the screen. For multi-block screens this is the number of blocks, horizontal and vertical.</returns>
        public async Task<(int Width, int Height)> GetAspectRatio()
        {
            var res = await GetInvoker()();
            return (res[0], res[1]);
        }

        /// <returns>Whether the screen is currently on.</returns>
        public partial Task<bool> IsOn();

		/// <summary>
		/// Turns the screen on.
		/// </summary>
		/// <returns>true if it was off.</returns>
		public partial Task<bool> TurnOn();

        /// <summary>
        /// Turns off the screen.
        /// </summary>
        /// <returns>true if it was on.</returns>
        public partial Task<bool> TurnOff();

        /// <returns>The list of keyboards attached to the screen.</returns>
        public async Task<IEnumerable<KeyboardComponent>> GetKeyboards()
        {
            var comps = await Parent.GetComponents();
            return (await GetInvoker()()).Linq.Select(x => comps.Get<KeyboardComponent>(Guid.Parse(x.Value.Value))).Where(x => x is not null);
        }

        /// <returns>Whether the screen is in high precision mode (sub-pixel mouse event positions).</returns>
        public partial Task<bool> IsPrecise();

        /// <param name="precise">Set whether to use high precision mode (sub-pixel mouse event positions).</param>
        /// <returns>The old value</returns>
        public async Task<bool> SetPrecise(bool precise)
        {
            if (await GetTier() < Tier.Three) throw new NotSupportedException("Precise mode is not supported on this screen");
            return (await GetInvoker()(precise))[0];
        }

        /// <returns>Whether touch mode is inverted (sneak-activate opens GUI, instead of normal activate).</returns>
        public partial Task<bool> IsTouchModeInverted();

        /// <summary>
        /// Sets whether to invert touch mode (sneak-activate opens GUI, instead of normal activate).
        /// </summary>
        /// <param name="inverted">Touch mode</param>
        /// <returns>The old value</returns>
        public partial Task<bool> SetTouchModeInverted(bool inverted);

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
