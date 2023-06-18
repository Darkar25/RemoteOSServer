using RemoteOS.OpenComputers.Data;

namespace RemoteOS.Helpers
{
    /// <summary>
    /// Set a tier for the component
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TierAttribute : Attribute
    {
        public Tier Tier { get; init; }
        public TierAttribute(Tier tier) => Tier = tier;
    }
}
