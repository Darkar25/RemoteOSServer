using EasyJSON;

namespace RemoteOS.OpenComputers.Components
{
    [Component("tunnel")]
    public class TunnelComponent : Component
    {
        public TunnelComponent(Machine parent, Guid address) : base(parent, address)
        {
        }
        int? _maxPacketSize;
        string? _channel;
        string? _wakeMessage;
        bool? _wakeFuzzy;
        public async Task Send(params JSONNode[] args) => await Invoke("send", args);
        public async Task<(string Message, bool Fuzzy)> GetWakeMessage()
        {
            if (_wakeMessage is null || _wakeFuzzy is null)
            {
                var res = await Invoke("getWakeMessage");
                _wakeMessage = res[0];
                _wakeFuzzy = res[1];
            }
            return (_wakeMessage, _wakeFuzzy.Value);
        }
        public async Task SetWakeMessage(string message, bool fuzzy = false)
        {
            _wakeMessage = message;
            _wakeFuzzy = fuzzy;
            await Invoke("setWakeMessage", $@"""{_wakeMessage}""", _wakeFuzzy);
        }
        public async Task<int> GetMaxPacketSize() => _maxPacketSize ??= (await Invoke("maxPacketSize"))[0];
        public async Task<string> GetChannel() => _channel ??= (await Invoke("getChannel"))[0];

#if ROS_PROPERTIES
        public (string Message, bool Fuzzy) WakeMessage
        {
            get => GetWakeMessage().Result;
            set => SetWakeMessage(value.Message, value.Fuzzy);
        }
        public int MaxPacketSize => GetMaxPacketSize().Result;
        public string Channel => GetChannel().Result;
#endif
    }
}
