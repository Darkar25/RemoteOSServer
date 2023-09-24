using EasyJSON;
using RemoteOS.OpenComputers;
using RemoteOS.OpenComputers.Data;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("tunnel")]
    [Tier(Tier.Three)]
    public partial class TunnelComponent : Component
    {
        public TunnelComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        int? _maxPacketSize;
        string? _channel;
        string? _wakeMessage;
        bool? _wakeFuzzy;

        /// <summary>
        /// Sends the specified data to the card this one is linked to.
        /// </summary>
        /// <param name="args">The data to send</param>
        public partial Task Send(params JSONNode[] args);

        /// <returns>The current wake-up message.</returns>
        public async Task<(string Message, bool Fuzzy)> GetWakeMessage()
        {
            if (_wakeMessage is null || _wakeFuzzy is null)
            {
                var res = await GetInvoker()();
                _wakeMessage = res[0];
                _wakeFuzzy = res[1];
            }
            return (_wakeMessage, _wakeFuzzy.Value);
        }

        /// <summary>
        /// Set the wake-up message and whether to ignore additional data/parameters.
        /// </summary>
        /// <param name="message">Wake message</param>
        /// <param name="fuzzy">Ignore data</param>
        public Task SetWakeMessage(string message, bool fuzzy = false)
        {
            _wakeMessage = message;
            _wakeFuzzy = fuzzy;
            return GetInvoker()(_wakeMessage, _wakeFuzzy);
        }

        /// <returns>The maximum packet size (config setting).</returns>
        public async Task<int> GetMaxPacketSize() => _maxPacketSize ??=
#if ROS_GLOBAL_CACHING
            GlobalCache.maxNetworkPacketSize ??= 
#endif
            (await Invoke("maxPacketSize"))[0];

        /// <returns>This link card's shared channel address</returns>
        public async Task<string> GetChannel() => _channel ??= (await GetInvoker()())[0];

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
