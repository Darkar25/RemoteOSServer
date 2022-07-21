using System.Linq;
using EasyJSON;

namespace RemoteOS.OpenComputers.Components
{
    [Component("modem")]
    public class ModemComponent : Component
    {
        bool? _isWireless;
        bool? _isWired;
        int? _maxPacketSize;
        int? _strength;
        string? _wakeMessage;
        List<int> _openPorts = new();

        public event Action<Guid, int, int, IEnumerable<JSONNode>>? ModemMessage;

        public ModemComponent(Machine parent, Guid address) : base(parent, address)
        {
            parent.Listen("modem_message", (parameters) => {
                if (parameters[0].Value == Address.ToString())
                    ModemMessage?.Invoke(Guid.Parse(parameters[1]), parameters[2], parameters[3], parameters.Linq.Skip(4).Select(x => x.Value));
            });
        }
        
        public bool IsOpen(int port) => _openPorts.Contains(port);
        public async Task<bool> Open(int port)
        {
            var res = await Invoke("open", port);
            if (res[0])
                _openPorts.Add(port);
            return res[0];
        }

        public async Task<bool> Close(int port)
        {
            var res = await Invoke("close", port);
            if (res[0])
                _openPorts.Remove(port);
            return res[0];
        }

        public async Task<bool> Close()
        {
            var res = await Invoke("close");
            if (res[0])
                _openPorts.Clear();
            return res[0];
        }

        public async Task<bool> Send(Guid reciever, int port, params JSONNode[] args) => (await Invoke("send", $@"""{reciever}""", port, args))[0];
        public async Task<bool> Broadcast(int port, params JSONNode[] args) => (await Invoke("broadcast", port, args))[0];
        public async Task<string> SetWakeMessage(string message, bool fuzzy = false) => _wakeMessage = (await Invoke("setWakeMessage", $@"""{message}""", fuzzy))[0];
        public async Task<bool> IsWireless() => _isWireless ??= (await Invoke("isWireless"))[0];
        public async Task<bool> IsWired() => _isWired ??= (await Invoke("isWired"))[0];
        public async Task<int> GetMaxPacketSize() => _maxPacketSize ??= (await Invoke("maxPacketSize"))[0];
        public async Task<int> GetStrength() => _strength ??= (await Invoke("getStrength"))[0];
        public async Task SetStrength(int strength) => _strength = (await Invoke("setStrength", strength))[0];
        public async Task<string> GetWakeMessage() => _wakeMessage ??= (await Invoke("getWakeMessage"))[0];

        public bool this[int port]
        {
            get => IsOpen(port);
            set
            {
                if (value)
                    Open(port);
                else
                    Close(port);
            }
        }

#if ROS_PROPERTIES
        public bool Wireless => IsWireless().Result;
        public bool Wired => IsWired().Result;
        public int MaxPacketSize => GetMaxPacketSize().Result;
        public int Strength
        {
            get => GetStrength().Result;
            set => SetStrength(value);
        }
        public string WakeMessage => GetWakeMessage().Result;
#endif
    }
}
