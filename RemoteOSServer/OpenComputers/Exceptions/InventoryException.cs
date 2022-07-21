namespace RemoteOS.OpenComputers.Exceptions
{
    public class InventoryException : Exception
    {
        public const string NO_SUCH_SLOT = "This slot does not exist";
        public const string NO_SUCH_TANK = "No such tank";
        public InventoryException() : base() { }
        public InventoryException(string message) : base(message) { }
    }
}
