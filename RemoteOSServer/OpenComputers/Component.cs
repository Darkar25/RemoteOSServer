using EasyJSON;
using System.Globalization;
using System.Reflection;

namespace RemoteOS.OpenComputers
{
    public abstract class Component
    {
        public Machine Parent { get; private set; }
        public Guid Address { get; private set; }
        string? _type;
        public string Type => _type ??= GetType().GetCustomAttribute<ComponentAttribute>().Codename;
        string? _handle;
        public string Handle {
            get {
                if (_handle == null)
                {
                    var count = 0;
                    while (Parent.Components.Any(x => x._handle == $"component.{Type}{count}")) count++;
                    Parent.RawExecute(@$"component.{Type}{count} = component.proxy(""{Address}"")").Wait();
                    _handle = $"component.{Type}{count}";
                }
                return _handle;
            }
        }
        int? _slot;
        public async Task<int> GetSlot() => _slot ??= (await Invoke("slot"))[0];
#if ROS_PROPERTIES
        public int Slot => GetSlot().Result;
#endif
        public Component(Machine parent, Guid address)
        {
            Parent = parent;
            Address = address;
        }
        public async Task<JSONNode> Invoke(string methodName, params object[] args)
        {
            List<string> parameters = new();
            foreach(var a in args)
            {
                parameters.Add(a.Luaify());
            }
            return await Parent.Execute($"{Handle}.{methodName}({string.Join(",", parameters)})");
        }
    }
}
