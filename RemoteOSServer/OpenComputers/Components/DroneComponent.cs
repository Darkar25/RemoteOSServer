namespace RemoteOS.OpenComputers.Components
{
    [Component("drone")]
    public class DroneComponent : Agent
    {
        /// <summary>
        /// This event is sent when the drone is hit by a entity
        /// <para>Parameters:
        /// <br>float - X velocity</br>
        /// <br>float - Y velocity</br>
        /// <br>float - Z velocity</br>
        /// <br>string - Name of the entity</br>
        /// </para>
        /// </summary>
        public event Action<float, float, float, string?> Hit;
        public DroneComponent(Machine parent, Guid address) : base(parent, address)
        {
            parent.Listen("hit", (parameters) => {
                Hit?.Invoke(parameters[0], parameters[1], parameters[2], parameters[3]);
            });
        }

        string? _statusText;
        double? _acceleration;

        /// <summary>
        /// The maximum velocity, in m/s.
        /// </summary>
        public double MaxVelocity => 8; //This is hardcoded in OC code

        /// <summary>
        /// Change the target position by the specified offset.
        /// </summary>
        /// <param name="dx">Relative X position</param>
        /// <param name="dy">Relative Y position</param>
        /// <param name="dz">Relative Z position</param>
        public async Task Move(float dx, float dy, float dz) => await Invoke("move", dx, dy, dz);
        /// <returns>The status text currently being displayed in the GUI.</returns>
        public async Task<string> GetStatusText() => _statusText ??= (await Invoke("getStatusText"))[0];
        /// <summary>
        /// Set the status text to display in the GUI.
        /// </summary>
        /// <param name="text">New status text value</param>
        /// <returns>The new status text value</returns>
        public async Task<string> SetStatusText(string text) => _statusText = (await Invoke("setStatusText", @$"""{text}"""))[0];
        /// <returns>The current distance to the target position.</returns>
        public async Task<double> GetOffset() => (await Invoke("getOffset"))[0];
        /// <returns>The current velocity in m/s.</returns>
        public async Task<double> GetVelocity() => (await Invoke("getVelocity"))[0];
        /// <returns>The currently set acceleration.</returns>
        public async Task<double> GetAcceleration() => _acceleration ??= (await Invoke("getAcceleration"))[0];
        /// <summary>
        /// Try to set the acceleration to the specified value.
        /// </summary>
        /// <param name="accel">New acceleration value</param>
        /// <returns>The new acceleration</returns>
        public async Task<double> SetAcceleration(double accel) => (_acceleration = (await Invoke("setAcceleration", accel))[0]).Value;
        
#if ROS_PROPERTIES
        public string StatusText
        {
            get => GetStatusText().Result;
            set => SetStatusText(value);
        }
        public double Acceleration
        {
            get => GetAcceleration().Result;
            set => SetAcceleration(value);
        }
#if ROS_PROPS_UNCACHED
        public double Offset => GetOffset().Result;
        public double Velocity => GetVelocity().Result;
#endif
#endif
    }
}
