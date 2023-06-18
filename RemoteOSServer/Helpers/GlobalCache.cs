#if ROS_GLOBAL_CACHING
using RemoteOS.OpenComputers.Data;

namespace RemoteOS.Helpers
{
    public class GlobalCache
    {
        public static int? dataCardHardLimit { get; set; }
        public static int? eepromSize { get; set; }
        public static int? eepromDataSize { get; set; }
        public static int? maxNetworkPacketSize { get; set; }
        public static Dictionary<Tier, int> vramSizes { get; } = new();
        public static Dictionary<Tier, int> gpuMaxWidth { get; } = new();
        public static Dictionary<Tier, int> gpuMaxHeight { get; } = new();
        public static float? geolyzerNoise { get; set; }
    }
}
#endif