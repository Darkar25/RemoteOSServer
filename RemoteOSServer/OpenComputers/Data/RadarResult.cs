namespace RemoteOSServer.OpenComputers.Data;

public class Entity
{
    public double Distance;
    public string Name;

    public Entity(string name, double distance)
    {
        Name = name;
        Distance = distance;
    }
}

public class Item
{
    public double Damage;
    public double Distance;
    public bool HasTag;
    public string Label;
    public double Size;

    public Item(double damage, double distance, bool hasTag, string label, double size)
    {
        Damage = damage;
        Distance = distance;
        HasTag = hasTag;
        Label = label;
        Size = size;
    }
}