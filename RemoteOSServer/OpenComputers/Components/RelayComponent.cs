namespace RemoteOS.OpenComputers.Components
{
    [Component("relay")]
    public class RelayComponent : Component
    {
        public RelayComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        public async Task<int> GetStrength() => (await Invoke("getStrength"))[0];
        public async Task SetStrength(int strength) => await Invoke("setStrength", strength);
        public async Task<bool> IsRepeater() => (await Invoke("isRepeater"))[0];
        public async Task SetRepeater(bool repeater) => await Invoke("setRepeater", repeater);

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public int Range
        {
            get => GetStrength().Result;
            set => SetStrength(value);
        }

        public bool Repeater
        {
            get => IsRepeater().Result;
            set => SetRepeater(value);
        }
#endif
    }
}
