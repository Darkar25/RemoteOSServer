namespace RemoteOS.OpenComputers.Components
{
    [Component("motion_sensor")]
    public class MotionSensorComponent : Component
    {
        public event Action<int, int, int, string?>? Motion;
        public MotionSensorComponent(Machine parent, Guid address) : base(parent, address)
        {
            parent.Listen("motion", (parameters) => {
                if (parameters[0].Value == Address.ToString())
                    Motion?.Invoke(parameters[1], parameters[2], parameters[3], parameters[4]);
            });
        }

        public async Task<int> GetSensitivity() => (await Invoke("getSensitivity"))[0];
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
