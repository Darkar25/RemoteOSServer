namespace RemoteOS.OpenComputers.Components
{
    [Component("drone")]
    public class DroneComponent : Agent
    {
        public event Action<float, float, float, string?> Hit;
        public DroneComponent(Machine parent, Guid address) : base(parent, address)
        {
            parent.Listen("hit", (parameters) => {
                Hit?.Invoke(parameters[0], parameters[1], parameters[2], parameters[3]);
            });
        }

        string? _statusText;
        double? _maxVel;
        double? _acceleration;

        public async Task Move(float dx, float dy, float dz) => await Invoke("move", dx, dy, dz);
        public async Task<string> GetStatusText() => _statusText ??= (await Invoke("getStatusText"))[0];
        public async Task SetStatusText(string text) => _statusText = (await Invoke("setStatusText", @$"""{text}"""))[0];
        public async Task<double> GetOffset() => (await Invoke("getOffset"))[0];
        public async Task<double> GetVelocity() => (await Invoke("getVelocity"))[0];
        public async Task<double> GetMaxVelocity() => _maxVel ??= (await Invoke("getMaxVelocity"))[0];
        public async Task<double> GetAcceleration() => _acceleration ??= (await Invoke("getAcceleration"))[0];
        public async Task SetAcceleration(double accel) => _acceleration = (await Invoke("setAcceleration", accel))[0];
#if ROS_PROPERTIES
        public string StatusText
        {
            get => GetStatusText().Result;
            set => SetStatusText(value);
        }
        public double MaxVelocity => GetMaxVelocity().Result;
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
