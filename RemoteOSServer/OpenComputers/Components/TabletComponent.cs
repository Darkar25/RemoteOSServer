using RemoteOS.OpenComputers.Data;
using System.Drawing;

namespace RemoteOS.OpenComputers.Components
{
    [Component("tablet")]
    public class TabletComponent : Component
    {
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

        public async Task<float> GetPitch() => (await Invoke("getPitch"))[0];
        public async Task<float> GetYaw() => (await Invoke("getYaw"))[0];

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public float Pitch => GetPitch().Result;
        public float Yaw => GetYaw().Result;
#endif
    }
}
