using System.Collections;

namespace RemoteOS.OpenComputers.Components.Computronics;

public class Entity
{
    public string Name;
    public double Distance;

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

