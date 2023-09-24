using RemoteOS.Helpers;
using System.Drawing;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("colorful_lamp")]
    public partial class ColorfulLampComponent : Component
    {
        public ColorfulLampComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <returns>The current lamp color</returns>
        public async Task<Color> GetLampColor() => Color.FromArgb((await GetInvoker()())[0]);

        /// <summary>
        /// Sets the lamp color; Set to 0 to turn the off the lamp;
        /// </summary>
        /// <param name="color">New color</param>
        /// <returns>True on success</returns>
        public async Task<bool> SetLampColor(Color color) => (await GetInvoker()(color))[0];

        /// <summary>
        /// Turns off the lamp
        /// </summary>
        /// <returns>True on success</returns>
        public Task<bool> TurnOff() => SetLampColor(Color.FromArgb(0));

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public Color Color {
            get => GetLampColor().Result;
            set => SetLampColor(value)
        }
#endif
    }
}