namespace RemoteOS.OpenComputers.Data
{
    public class OpenSecurityEntity : Entity
    {
        public int X;
        public int Y;
        public int Z;

        public OpenSecurityEntity(string name, double distance, int x, int y, int z) : base(name, distance)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}