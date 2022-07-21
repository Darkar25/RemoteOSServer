using System.Text.RegularExpressions;

namespace RemoteOS.OpenComputers.Components
{
    [Component("computer")]
    public class ComputerComponent : Component
    {
        public ComputerComponent(Machine parent, Guid address) : base(parent, address) { }
        public async Task<bool> Start() => (await Invoke("start"))[0];
        public async Task<bool> Stop() => (await Invoke("stop"))[0];
        public async Task Beep(int frequency, int duration = 0) => await Invoke("beep", frequency, duration);
        public async Task<bool> IsRunning() => (await Invoke("isRunning"))[0];
#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public bool Running => IsRunning().Result;
#endif
    }
}
