using RemoteOS.OpenComputers.Data;
using System.Drawing;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("tablet")]
    public partial class TabletComponent : Component
    {
        /// <summary>
        /// This event is sent when player uses tablet to analyze a block
        /// <para>Parameters:
        /// <br>GeolyzerResult - the result of analyze</br>
        /// </para>
        /// </summary>
        public event Action<GeolyzerResult>? TabletUse;
        public TabletComponent(Machine parent, Guid address) : base(parent, address)
        {
            parent.Listen("tablet_use", (parameters) => {
                TabletUse?.Invoke(new()
                {
                    Color = Color.FromArgb(parameters[0]["color"]),
                    Hardness = parameters[0]["hardness"],
                    HarvestLevel = parameters[0]["harvestLevel"],
                    HarvestTool = parameters[0]["harvestTool"],
                    Meta = parameters[0]["metadata"]
                });
            });
        }

        /// <returns>The pitch of the player holding the tablet.</returns>
        public partial Task<float> GetPitch();

        /// <returns>The yaw of the player holding the tablet.</returns>
        public partial Task<float> GetYaw();

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public float Pitch => GetPitch().Result;

        public float Yaw => GetYaw().Result;
#endif
    }
}
