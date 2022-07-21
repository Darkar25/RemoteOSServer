using EasyJSON;
using NetCoreServer;
using RemoteOS.OpenComputers;
using RemoteOS.OpenComputers.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RemoteOS
{
    public class RemoteOSServer : TcpServer
    {
        public delegate void MachineConnectionEvent(Machine machine);
        public MachineConnectionEvent? onConnected;
        public MachineConnectionEvent? onDisconnected;
        public RemoteOSServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() => new Machine(this);

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"RemoteOS server caught an error with code {error}");
        }
    }
}
