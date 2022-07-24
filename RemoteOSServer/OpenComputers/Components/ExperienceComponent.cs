namespace RemoteOS.OpenComputers.Components
{
    [Component("experience")]
    public class ExperienceComponent : Component
    {
        public ExperienceComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Tries to consume an enchanted item to add experience to the upgrade.
        /// </summary>
        /// <returns>Whether the consumption was successful and if it wasnt the reason why</returns>
        public async Task<(bool Success, string Reason)> Consume()
        {
            var res = await Invoke("consume");
            return (res[0], res[1]);
        }
        /// <returns>The current level of experience stored in this experience upgrade.</returns>
        public async Task<float> GetLevel() => (await Invoke("level"))[0];

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public float Level => GetLevel().Result;
#endif
    }
}
