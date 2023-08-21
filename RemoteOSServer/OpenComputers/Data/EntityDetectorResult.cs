namespace RemoteOSServer.OpenComputers.Data
{
    public class OsEntity : Entity
    {
        public int X;
        public int Y;
        public int Z;

        public OsEntity(string name, double distance, int x, int y, int z) : base(name, distance)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}