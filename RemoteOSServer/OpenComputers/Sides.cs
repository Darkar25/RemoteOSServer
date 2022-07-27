namespace RemoteOS.OpenComputers
{
    public enum Sides
    {
        Bottom,
        Top,
        Back,
        Front,
        Right,
        Left,
#if OpenOS
        // Aliases
        Down = Bottom,
        NegY = Bottom,
        Up = Top,
        PosY = Top,
        North = Back,
        NegZ = Back,
        South = Front,
        PosZ = Front,
        Forward = Front,
        West = Right,
        NegX = Right,
        East = Left,
        PosX = Left
#endif
    }
}
