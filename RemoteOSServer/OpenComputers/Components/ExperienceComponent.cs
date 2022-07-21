namespace RemoteOS.OpenComputers.Components
{
    [Component("experience")]
    public class ExperienceComponent : Component
    {
        public ExperienceComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        public async Task<(bool Success, string Reason)> Consume()
        {
            var res = await Invoke("consume");
            return (res[0], res[1]);
        }
        public async Task<float> GetLevel() => (await Invoke("level"))[0];

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public float Level => GetLevel().Result;
#endif
    }
}
