using RemoteOS.OpenComputers.Data;

namespace RemoteOS.OpenComputers.Exceptions
{
    public class InvalidSideException : Exception
    {
        public InvalidSideException() : base() { }
        public InvalidSideException(string message) : base(message) { }
        public InvalidSideException(params Sides[] validSides) : base(validSides.Length > 1 ? $"Valid sides are {string.Join(",", validSides[..^1])} and {validSides[^1]}" : $"The side can only be {validSides[0]}") { }
    }
}
