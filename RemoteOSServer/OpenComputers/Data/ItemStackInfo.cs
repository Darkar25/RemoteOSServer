using EasyJSON;
using System.Drawing;

namespace RemoteOS.OpenComputers.Data
{
    public struct ItemStackInfo
    {
        public int Durability;
        public int Meta;
        public int MaxDurability;
        public int Count;
        public int StackSize;
        public int Id;
        public string? Name;
        public string? ModName;
        public string Label;
        public bool HasTag;

        public static ItemStackInfo FromJson(JSONNode json) {
            var ret = new ItemStackInfo()
            {
                Count = json["size"],
                StackSize = json["maxSize"],
                Id = json["id"],
                Label = json["label"],
                HasTag = json["hasTag"],
                MaxDurability = json["maxDamage"]
            };
            int Damage = json["damage"];
            if (ret.MaxDurability == 0)
            {
                ret.Meta = Damage;
                ret.Durability = 0;
            }
            else
            {
                ret.Meta = 0;
                ret.Durability = ret.MaxDurability - Damage;
            }
            var n = json["name"].Value.Split(":");
            ret.ModName = n.FirstOrDefault();
            ret.Name = n.LastOrDefault();
            return ret;
        }
    }
}
