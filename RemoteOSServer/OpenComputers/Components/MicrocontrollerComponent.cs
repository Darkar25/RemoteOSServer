namespace RemoteOS.OpenComputers.Components
{
    [Component("microcontroller")]
    public class MicrocontrollerComponent : Component
    {
        public MicrocontrollerComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        public async Task<bool> SetSideOpen(Sides side, bool state) => (await Invoke("setSideOpen", side, state))[0];

        public async Task<bool> Start() => (await Invoke("start"))[0];

        public async Task<bool> Stop() => (await Invoke("stop"))[0];

        public async Task<bool> IsSideOpen(Sides side) => (await Invoke("isSideOpen", side))[0];

        public bool this[Sides side]
        {
            get => IsSideOpen(side).Result;
            set => SetSideOpen(side, value);
        }
        public async Task<bool> IsRunning() => (await Invoke("isRunning"))[0];
        public async Task<string> GetLastError() => (await Invoke("lastError"))[0];

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public bool Running => IsRunning().Result;
        public string LastError => GetLastError().Result;
#endif
    }
}
