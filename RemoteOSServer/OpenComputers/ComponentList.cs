using EasyJSON;
using RemoteOS.OpenComputers.Components;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace RemoteOS.OpenComputers
{
    public class ComponentList : IEnumerable<Component>
    {
        public Machine Parent { get; private set; }
        IEnumerable<Component> Components { get; set; }
        static Dictionary<string, Type> types = new();
        static ComponentList() {
            types = Assembly.GetExecutingAssembly().DefinedTypes.Where(x => x.GetCustomAttribute<ComponentAttribute>() is not null).ToDictionary(x => x.GetCustomAttribute<ComponentAttribute>().Codename, x => x.AsType());
        }
        Dictionary<Guid, string> addresses = new();

        public event Action<Component>? ComponentAdded;
        public event Action<Component>? ComponentRemoved;
        public ComponentList(Machine computer)
        {
            Parent = computer;
            Parent.Listen("component_added", (parameters) => {
                var guid = Guid.Parse(parameters[0]);
                addresses[guid] = parameters[1];
                if (types.ContainsKey(parameters[1]))
                {
                    var comp = Activator.CreateInstance(types[parameters[1]], Parent, guid) as Component;
                    Components = Components.Append(comp);
                    Parent.Computer.ClearDeviceInfoCache();
                    ComponentAdded?.Invoke(comp);
                }
            });
            Parent.Listen("component_removed", (parameters) => {
                var guid = Guid.Parse(parameters[0]);
                var comp = Get(guid);
                if (comp is not null)
                {
                    addresses.Remove(guid);
                    Components = Components.Where(x => x != comp);
                    Parent.Computer.ClearDeviceInfoCache();
                    ComponentRemoved?.Invoke(comp);
                }
            });
        }
        internal async Task LoadAsync()
        {
            addresses = (await Parent.Execute($"component.list()"))[0].Linq.ToDictionary(x => Guid.Parse(x.Key), x => x.Value.Value);
            Components = addresses.Where(x => types.ContainsKey(x.Value)).Select(x => Activator.CreateInstance(types[x.Value], Parent, x.Key) as Component);
        }
        public IEnumerator<Component> GetEnumerator() => List().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool IsAvailable<T>() where T : Component => List<T>().Any();
        public IEnumerable<Component> List() => Components;
        public IEnumerable<T> List<T>() where T : Component => List().OfType<T>();
        public IEnumerable<Component> List(string filter, bool exact = false) => List().Where(x => exact ? x.Type == filter : x.Type.Contains(filter));
        public T? GetPrimary<T>() where T : Component
        {
            if(typeof(T) == typeof(ComputerComponent))
                return Get<T>(Parent.Computer.Address);
            else
                return List<T>().FirstOrDefault();
        }

        public Component? Get(Guid address) => List().FirstOrDefault(x => x.Address == address);
        public T? Get<T>(Guid address) where T : Component => List<T>().FirstOrDefault(x => x.Address == address);
        public async Task<JSONNode> Invoke(Component component, string methodName, params object[] args) => await component.Invoke(methodName, args);
    }
}
