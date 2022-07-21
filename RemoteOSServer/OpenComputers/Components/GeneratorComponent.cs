namespace RemoteOS.OpenComputers.Components
{
    [Component("generator")]
    public class GeneratorComponent : Component
    {
        public GeneratorComponent(Machine parent, Guid address) : base(parent, address)
        {
        }
        public async Task<(bool Success, string Reason)> Insert(int count = 64)
        {
            var res = await Invoke("insert", count);
            return (res[0], res[1]);
        }
        public async Task<bool> Remove(int count = 64) => (await Invoke("remove", count))[0];
        public async Task<int> GetCount() => (await Invoke("count"))[0];

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public int Count => GetCount().Result;
#endif
    }
}
