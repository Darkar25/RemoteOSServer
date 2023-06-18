using System.Linq;
using EasyJSON;
using RemoteOS.OpenComputers;
using RemoteOS.OpenComputers.Data;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("modem")]
    public partial class ModemComponent : Component
    {
        bool? _isWireless;
        bool? _isWired;
        int? _maxPacketSize;
        int? _strength;
        string? _wakeMessage;
        List<ushort> _openPorts = new();

        /// <summary>
        /// This event is sent when modem receives a packet
        /// <para>Parameters:
        /// <br>Guid - sender address</br>
        /// <br>ushort - port</br>
        /// <br>int - distance</br>
        /// <br>IEnumerable&lt;JSONNode&gt; - data</br>
        /// </para>
        /// </summary>
        public event Action<Guid, ushort, int, IEnumerable<JSONNode>>? ModemMessage;

        public ModemComponent(Machine parent, Guid address) : base(parent, address)
        {
            parent.Listen("modem_message", (parameters) => {
                if (parameters[0].Value == Address.ToString())
                    ModemMessage?.Invoke(Guid.Parse(parameters[1]), (ushort)parameters[2].AsInt, parameters[3], parameters.Linq.Skip(4).Select(x => x.Value));
            });
        }

        public override async Task<Tier> GetTier()
        {
            var ver = (await this.GetDeviceInfo()).Version;
            return (Tier)int.Parse(ver[..ver.IndexOf('.')]) - 1;
        }

        /// <param name="port">Port to check</param>
        /// <returns>Whether the specified port is open.</returns>
        public bool IsOpen(ushort port) => _openPorts.Contains(port);

        /// <summary>
        /// Opens the specified port.
        /// </summary>
        /// <param name="port">Port to open</param>
        /// <returns>true if the port was opened.</returns>
        public async Task<bool> Open(ushort port)
        {
            if (_openPorts.Contains(port)) return false;
            if (_openPorts.Count >= (await this.GetDeviceInfo()).Size) throw new IOException("Too many open ports");
            var res = await InvokeFirst("open", port);
            if (res) _openPorts.Add(port);
            return res;
        }

        /// <summary>
        /// Closes the specified port.
        /// </summary>
        /// <param name="port">Port to close</param>
        /// <returns>true if ports were closed.</returns>
        public async Task<bool> Close(ushort port)
        {
            var res = await InvokeFirst("close", port);
            if (res) _openPorts.Remove(port);
            return res;
        }

        /// <summary>
        /// Closes all ports.
        /// </summary>
        /// <returns>true if ports were closed.</returns>
        public async Task<bool> Close()
        {
            var res = await InvokeFirst("close");
            if (res) _openPorts.Clear();
            return res;
        }

        /// <summary>
        /// Sends the specified data to the specified target.
        /// </summary>
        /// <param name="reciever">Address of the receiver</param>
        /// <param name="port">Port of the receiver</param>
        /// <param name="args">Data to send</param>
        /// <returns>true if data was sent</returns>
        public partial Task<bool> Send(Guid reciever, ushort port, params JSONNode[] args);

        /// <inheritdoc cref="Send(Guid, ushort, JSONNode[])"/>
        public Task<bool> Send(ModemComponent reciever, ushort port, params JSONNode[] args) => Send(reciever.Address, port, args);

        /// <summary>
        /// Broadcasts the specified data on the specified port.
        /// </summary>
        /// <param name="port">Port to broadcast on</param>
        /// <param name="args">Data to send</param>
        /// <returns>true if data was sent</returns>
        public partial Task<bool> Broadcast(ushort port, params JSONNode[] args);

        /// <summary>
        /// Set the wake-up message and whether to ignore additional data/parameters.
        /// </summary>
        /// <param name="message">Wake message</param>
        /// <param name="fuzzy">Ignore data</param>
        /// <returns>The old value</returns>
        public async Task<string> SetWakeMessage(string message, bool fuzzy = false)
        {
            var res = await InvokeFirst("setWakeMessage", message, fuzzy);
			_wakeMessage = message;
            return res;
		}

        /// <returns>Whether this card has wireless networking capability.</returns>
        public async Task<bool> IsWireless() => _isWireless ??= await InvokeFirst("isWireless");

        /// <returns>Whether this card has wired networking capability.</returns>
        public async Task<bool> IsWired() => _isWired ??= await InvokeFirst("isWired");

        /// <returns>The maximum packet size (config setting).</returns>
        public async Task<int> GetMaxPacketSize() => _maxPacketSize ??=
#if ROS_GLOBAL_CACHING
            GlobalCache.maxNetworkPacketSize ??= 
#endif
            await InvokeFirst("maxPacketSize");

        /// <returns>The signal strength (range) used when sending messages.</returns>
        public async Task<int> GetStrength() => _strength ??= await InvokeFirst("getStrength");

        /// <summary>
        /// Set the signal strength (range) used when sending messages.
        /// </summary>
        /// <param name="strength">New strength value</param>
        public async Task SetStrength(int strength) => _strength = await InvokeFirst("setStrength", strength);

        /// <returns>The current wake-up message.</returns>
        public async Task<string> GetWakeMessage() => _wakeMessage ??= await InvokeFirst("getWakeMessage");

        public bool this[ushort port]
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
