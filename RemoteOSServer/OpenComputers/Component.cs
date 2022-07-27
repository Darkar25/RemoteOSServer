using EasyJSON;
using System.Globalization;
using System.Reflection;

namespace RemoteOS.OpenComputers
{
    public abstract class Component
    {
        int? _slot;
        string? _type;
        string? _handle;

        public Component(Machine parent, Guid address)
        {
            Parent = parent;
            Address = address;
        }

        public Machine Parent { get; private set; }
        public Guid Address { get; private set; }
        public string Type => _type ??= GetType().GetCustomAttribute<ComponentAttribute>().Codename;
        public async Task<string> GetHandle()
        {
            if (_handle == null)
            {
                var count = 0;
                while ((await Parent.GetComponents()).Any(x => x._handle == $"component.{Type}{count}")) count++;
                await Parent.RawExecute(@$"component.{Type}{count} = component.proxy(""{Address}"")");
                _handle = $"component.{Type}{count}";
            }
            return _handle;
        }

        public async Task<JSONNode> Invoke(string methodName, params object[] args)
        {
            List<string> parameters = new();
            foreach (var a in args)
            {
                parameters.Add(a.Luaify());
            }
            return await Parent.Execute($"{await GetHandle()}.{methodName}({string.Join(",", parameters)})");
        }

        public async Task<int> GetSlot() => _slot ??= (await Invoke("slot"))[0];

#if ROS_PROPERTIES
        public int Slot => GetSlot().Result;
#endif
    }
}
