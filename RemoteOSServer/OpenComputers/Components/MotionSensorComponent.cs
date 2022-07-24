namespace RemoteOS.OpenComputers.Components
{
    [Component("motion_sensor")]
    public class MotionSensorComponent : Component
    {
        /// <summary>
        /// This event is sent when motion sensor detects movement
        /// <para>Parameters:
        /// <br>int - relative x position</br>
        /// <br>int - relative y position</br>
        /// <br>int - relative z position</br>
        /// <br>string - entity name</br>
        /// </para>
        /// </summary>
        public event Action<int, int, int, string?>? Motion;
        public MotionSensorComponent(Machine parent, Guid address) : base(parent, address)
        {
            parent.Listen("motion", (parameters) => {
                if (parameters[0].Value == Address.ToString())
                    Motion?.Invoke(parameters[1], parameters[2], parameters[3], parameters[4]);
            });
        }

        /// <returns>The current sensor sensitivity.</returns>
        public async Task<int> GetSensitivity() => (await Invoke("getSensitivity"))[0];
        /// <summary>
        /// Sets the sensor's sensitivity.
        /// </summary>
        /// <param name="sensitivity">The new sensitivity value</param>
        /// <returns>The old value</returns>
        public async Task SetSensitivity(int sensitivity) => await Invoke("setSensitivity", sensitivity);

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public int Sensitivity
        {
            get => GetSensitivity().Result;
            set => SetSensitivity(value);
        }
#endif
    }
}
