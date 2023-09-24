using RemoteOS.Helpers;
using RemoteOS.OpenComputers.Data;

namespace RemoteOS.OpenComputers.Components
{
    [Component("microcontroller")]
    public partial class MicrocontrollerComponent : Component
    {
        public MicrocontrollerComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Set whether network messages are sent via the specified side.
        /// </summary>
        /// <param name="side">The side to change state of</param>
        /// <param name="state">New state</param>
        /// <returns>The old value</returns>
        public partial Task<bool> SetSideOpen(Sides side, bool state);

        /// <summary>
        /// Starts the microcontroller.
        /// </summary>
        /// <returns>true if the state changed.</returns>
        public partial Task<bool> Start();

        /// <summary>
        /// Stops the microcontroller.
        /// </summary>
        /// <returns>true if the state changed.</returns>
        public partial Task<bool> Stop();

        /// <param name="side">The side to get state of</param>
        /// <returns>Whether network messages are sent via the specified side.</returns>
        public partial Task<bool> IsSideOpen(Sides side);

        public bool this[Sides side]
        {
            get => IsSideOpen(side).Result;
            set => SetSideOpen(side, value);
        }
        /// <returns>Whether the microcontroller is running.</returns>
        public partial Task<bool> IsRunning();

        /// <returns>The reason the microcontroller crashed, if applicable.</returns>
        public async Task<string> GetLastError() => (await Invoke("lastError"))[0];

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public bool Running => IsRunning().Result;

        public string LastError => GetLastError().Result;
#endif
    }
}
