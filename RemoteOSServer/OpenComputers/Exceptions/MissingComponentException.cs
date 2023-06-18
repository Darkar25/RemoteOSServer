using System.Reflection;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Exceptions
{
    public class MissingComponentException<TComponent> : Exception where TComponent : Component
    {
        public MissingComponentException() : this($"Component \"{typeof(TComponent).GetCustomAttribute<ComponentAttribute>()?.Codename ?? typeof(TComponent).Name}\" is required, but not present on the machine.") { }
        public MissingComponentException(string message) : base(message) { }
    }
}
