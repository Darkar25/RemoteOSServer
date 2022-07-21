using EasyJSON;
using System.Collections.ObjectModel;
using System.Drawing;

namespace RemoteOS.OpenComputers.Data
{
    public struct GeolyzerResult
    {
        public Color Color;
        public float Hardness;
        public int HarvestLevel;
        public string HarvestTool;
        public int Meta;
        public string Name;
        public ReadOnlyDictionary<string, JSONNode> Properties;
    }
}
