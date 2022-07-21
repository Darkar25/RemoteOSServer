using EasyJSON;

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
        public ItemStackInfo(JSONNode data)
        {
            int Damage = data["damage"];
            MaxDurability = data["maxDamage"];
            if (MaxDurability == 0)
            {
                Meta = Damage;
                Durability = 0;
            } else
            {
                Meta = 0;
                Durability = MaxDurability - Damage;
            }
            Count = data["size"];
            StackSize = data["maxSize"];
            Id = data["id"];
            var n = data["name"].Value.Split(":");
            ModName = n.FirstOrDefault();
            Name = n.LastOrDefault();
            Label = data["label"];
            HasTag = data["hasTag"];
        }
    }
}
