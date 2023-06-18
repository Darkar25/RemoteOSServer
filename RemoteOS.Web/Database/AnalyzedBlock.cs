using EasyJSON;
using Newtonsoft.Json;
using RemoteOS.OpenComputers.Data;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace RemoteOS.Web.Database
{
    public class AnalyzedBlock : ScannedBlock
    {
        public AnalyzedBlock() : base() { }
        public AnalyzedBlock(GeolyzerResult geolyzerResult) : base() {
            Color = geolyzerResult.Color;
            HarvestLevel = geolyzerResult.HarvestLevel;
            HarvestTool = geolyzerResult.HarvestTool;
            Hardness = geolyzerResult.Hardness;
            Meta = geolyzerResult.Meta;
            Properties = geolyzerResult.Properties;
            var split = geolyzerResult.Name.Split(':', 2);
            ModName = split[0];
            Name = split[1];
        }
        public int Argb { get; set; }
        [NotMapped]
        public Color Color
        {
            get => Color.FromArgb(Argb);
            set => Argb = value.ToArgb();
        }
        public int HarvestLevel { get; set; }
        public string HarvestTool { get; set; }
        public int Meta { get; set; }
        public string ModName { get; set; }
        public string Name { get; set; }
        [NotMapped]
        [JsonIgnore]
        public ReadOnlyDictionary<string, JSONNode>? Properties { get; set; }
        public string Props {
            get
            {
                if (Properties is null) return "{}";
                var ret = new JSONObject();
                foreach (var a in Properties)
                    ret[a.Key] = a.Value;
                return ret.ToString();
            }
            set
            {
                Properties = new(JSON.Parse(value).Linq.ToDictionary(x => x.Key, x => x.Value));
            }
        }
    }
}
