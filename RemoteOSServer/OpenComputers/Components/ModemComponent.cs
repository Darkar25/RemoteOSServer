using System.Linq;
using EasyJSON;
using RemoteOS.OpenComputers;

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

        /// <summary>
        /// This event is sent when modem receives a packet
        /// <para>Parameters:
        /// <br>Guid - sender address</br>
        /// <br>int - port</br>
        /// <br>int - distance</br>
        /// <br>IEnumerable&lt;JSONNode&gt; - data</br>
        /// </para>
        /// </summary>
        public event Action<Guid, int, int, IEnumerable<JSONNode>>? ModemMessage;

        public ModemComponent(Machine parent, Guid address) : base(parent, address)
        {
            parent.Listen("modem_message", (parameters) => {
                if (parameters[0].Value == Address.ToString())
                    ModemMessage?.Invoke(Guid.Parse(parameters[1]), parameters[2], parameters[3], parameters.Linq.Skip(4).Select(x => x.Value));
            });
        }

        /// <param name="port">Port to check</param>
        /// <returns>Whether the specified port is open.</returns>
        public bool IsOpen(int port) => _openPorts.Contains(port);
        /// <summary>
        /// Opens the specified port.
        /// </summary>
        /// <param name="port">Port to open</param>
        /// <returns>true if the port was opened.</returns>
        public async Task<bool> Open(int port)
        {
            var res = await Invoke("open", port);
            if (res[0])
                _openPorts.Add(port);
            return res[0];
        }
        /// <summary>
        /// Closes the specified port.
        /// </summary>
        /// <param name="port">Port to close</param>
        /// <returns>true if ports were closed.</returns>
        public async Task<bool> Close(int port)
        {
            var res = await Invoke("close", port);
            if (res[0])
                _openPorts.Remove(port);
            return res[0];
        }
        /// <summary>
        /// Closes all ports.
        /// </summary>
        /// <returns>true if ports were closed.</returns>
        public async Task<bool> Close()
        {
            var res = await Invoke("close");
            if (res[0])
                _openPorts.Clear();
            return res[0];
        }
        /// <summary>
        /// Sends the specified data to the specified target.
        /// </summary>
        /// <param name="reciever">Address of the receiver</param>
        /// <param name="port">Port of the receiver</param>
        /// <param name="args">Data to send</param>
        /// <returns>true if data was sent</returns>
        public async Task<bool> Send(Guid reciever, int port, params JSONNode[] args) => (await Invoke("send", $@"""{reciever}""", port, args))[0];
        /// <summary>
        /// Broadcasts the specified data on the specified port.
        /// </summary>
        /// <param name="port">Port to broadcast on</param>
        /// <param name="args">Data to send</param>
        /// <returns>true if data was sent</returns>
        public async Task<bool> Broadcast(int port, params JSONNode[] args) => (await Invoke("broadcast", port, args))[0];
        /// <summary>
        /// Set the wake-up message and whether to ignore additional data/parameters.
        /// </summary>
        /// <param name="message">Wake message</param>
        /// <param name="fuzzy">Ignore data</param>
        /// <returns>The old value</returns>
        public async Task<string> SetWakeMessage(string message, bool fuzzy = false) => _wakeMessage = (await Invoke("setWakeMessage", $@"""{message}""", fuzzy))[0];
        /// <returns>Whether this card has wireless networking capability.</returns>
        public async Task<bool> IsWireless() => _isWireless ??= (await Invoke("isWireless"))[0];
        /// <returns>Whether this card has wired networking capability.</returns>
        public async Task<bool> IsWired() => _isWired ??= (await Invoke("isWired"))[0];
        /// <returns>The maximum packet size (config setting).</returns>
        public async Task<int> GetMaxPacketSize() => _maxPacketSize ??=
#if ROS_GLOBAL_CACHING
            GlobalCache.maxNetworkPacketSize ??= 
#endif
            (await Invoke("maxPacketSize"))[0];
        /// <returns>The signal strength (range) used when sending messages.</returns>
        public async Task<int> GetStrength() => _strength ??= (await Invoke("getStrength"))[0];
        /// <summary>
        /// Set the signal strength (range) used when sending messages.
        /// </summary>
        /// <param name="strength">New strength value</param>
        public async Task SetStrength(int strength) => _strength = (await Invoke("setStrength", strength))[0];
        /// <returns>The current wake-up message.</returns>
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
