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

        public static GeolyzerResult FromJson(JSONNode json) => new()
        {
            Color = Color.FromArgb(json["color"]),
            Hardness = json["hardness"],
            HarvestLevel = json["harvestLevel"],
            HarvestTool = json["harvestTool"],
            Meta = json["metadata"],
            Name = json["name"],
            Properties = new(json["properties"].Linq.ToDictionary(x => x.Key, x => x.Value))
        };
    }
}
