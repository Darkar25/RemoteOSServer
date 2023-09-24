using OneOf;
using OneOf.Types;
using RemoteOS.OpenComputers.Data;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("experience")]
    [Tier(Tier.Three)]
    public class ExperienceComponent : Component
    {
        public ExperienceComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Tries to consume an enchanted item to add experience to the upgrade.
        /// </summary>
        /// <returns>Whether the consumption was successful and if it wasnt the reason why</returns>
        public async Task<ReasonOr<Success>> Consume()
        {
            var res = await GetInvoker()();
            if (!res[0]) return new Reason(res[1].Value);
            return new Success();
        }

        /// <returns>The current level of experience stored in this experience upgrade.</returns>
        public async Task<float> GetLevel() => (await Invoke("level"))[0];

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public float Level => GetLevel().Result;
#endif
    }
}
