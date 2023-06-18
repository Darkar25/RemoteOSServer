using EasyJSON;
using RemoteOS.OpenComputers.Components;
using System.Collections;
using System.Linq;
using System.Reflection;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers
{
    public class ComponentList : IEnumerable<Component>
    {
        public Machine Parent { get; private set; }
        IList<Component> Components { get; set; } = null!;
        static Dictionary<string, Type> types = new();
        static ComponentList() {
            types = Assembly.GetExecutingAssembly().DefinedTypes.Where(x => x.GetCustomAttribute<ComponentAttribute>() is not null).ToDictionary(x => x.GetCustomAttribute<ComponentAttribute>()!.Codename, x => x.AsType());
        }
        Dictionary<Guid, string> addresses = new();

        /// <summary>
        /// This event is sent when component is added to the machine
        /// <para>Parameters:
        /// <br>Component - the component that was added</br>
        /// </para>
        /// </summary>
        public event Action<Component>? ComponentAdded;
        /// <summary>
        /// This event is sent when component is romeved from the machine
        /// <para>Parameters:
        /// <br>Component - the component that was removed</br>
        /// </para>
        /// </summary>
        public event Action<Component>? ComponentRemoved;

        public ComponentList(Machine computer)
        {
            Parent = computer;
            Parent.Listen("component_added", (parameters) => {
                var guid = Guid.Parse(parameters[0]);
                addresses[guid] = parameters[1];
                if (types.ContainsKey(parameters[1]))
                {
                    var comp = (Component)Activator.CreateInstance(types[parameters[1]], Parent, guid)!;
                    Components!.Add(comp);
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
                    Components!.Remove(comp);
                    Parent.Computer.ClearDeviceInfoCache();
                    ComponentRemoved?.Invoke(comp);
                }
            });
        }

        internal async Task<ComponentList> LoadAsync()
        {
            addresses = (await Parent.Execute($"component.list()"))[0].Linq.ToDictionary(x => Guid.Parse(x.Key), x => x.Value.Value);
            Components = addresses.Where(x => types.ContainsKey(x.Value)).Select(x => (Component)Activator.CreateInstance(types[x.Value], Parent, x.Key)!).ToList();
            return this;
        }

        public IEnumerator<Component> GetEnumerator() => List().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <typeparam name="T">The component to find</typeparam>
        /// <returns>Wheter the specified component is available</returns>
        public bool IsAvailable<T>() where T : Component => List<T>().Any();

        /// <returns>All available components</returns>
        public IEnumerable<Component> List() => Components;

        /// <typeparam name="T">Component type</typeparam>
        /// <returns>All available components of the specified type</returns>
        public IEnumerable<T> List<T>() where T : Component => List().OfType<T>();

        /// <summary>
        /// Finds components using specified filter
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="exact">Component must match the filter exactly</param>
        /// <returns>All found components</returns>
        public IEnumerable<Component> List(string filter, bool exact = false) => List().Where(x => exact ? x.Type == filter : x.Type.Contains(filter, StringComparison.InvariantCultureIgnoreCase));
        
        /// <typeparam name="T">Which component to get</typeparam>
        /// <returns>The primary component of the specified type</returns>
        public T? GetPrimary<T>() where T : Component
        {
            if(typeof(T) == typeof(ComputerComponent))
                return Get<T>(Parent.Computer.GetAddress().Result);
            else
                return List<T>().FirstOrDefault();
        }

        /// <param name="address">The address to find</param>
        /// <returns>Component that has the specified address</returns>
        public Component? Get(Guid address) => List().FirstOrDefault(x => x.Address == address);

        /// <param name="address">The address to find</param>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <returns>Component that has the specified address and type</returns>
        public T? Get<T>(Guid address) where T : Component => List<T>().FirstOrDefault(x => x.Address == address);
    }
}
