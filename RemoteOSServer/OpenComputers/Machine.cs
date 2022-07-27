using EasyJSON;
using NetCoreServer;
using RemoteOS.OpenComputers.Exceptions;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace RemoteOS.OpenComputers
{
    public class Machine : TcpSession
    {
        protected new RemoteOSServer Server { get; set; }
        protected ConcurrentDictionary<string, TaskCompletionSource<string[]>> executequeue = new();
        Dictionary<string, Action<JSONArray>> SignalEvnets = new();
        ComponentList? _components;
        public Machine(RemoteOSServer server) : base(server)
        {
            Server = server;
            Computer = new(this);
        }

        protected override void OnConnected()
        {
            new Task(() => Server.onConnected?.Invoke(this), TaskCreationOptions.LongRunning).Start();
        }

        protected override void OnDisconnected()
        {
            new Task(() => Server.onDisconnected?.Invoke(this), TaskCreationOptions.LongRunning).Start();
        }

        public void Listen(string signal, Action<JSONArray> action)
        {
            if(SignalEvnets.ContainsKey(signal)) SignalEvnets[signal] += action;
            else SignalEvnets[signal] = action;
        }

        public void FireSignal(string signal, params JSONNode[] parameters)
        {
            if (SignalEvnets.TryGetValue(signal, out var act))
                act(new(parameters));
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            var b = message.Split("\r\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var msg in b)
            {
                var a = msg.Split('\0');
                switch (a[0])
                {
                    case "e":
                        var e = JSONNode.Parse(a[1]).AsArray;
                        var name = e[0];
                        e.Remove(0);
                        if (SignalEvnets.TryGetValue(name, out var act))
                            act(e);
                        break;
                    case "r":
                        if (executequeue.TryGetValue(a[1], out var task))
                        {
                            task.SetResult(a[2..]);
                            executequeue.Remove(a[1], out _);
                        }
                        break;
                    default:
                        Debug.WriteLine("Unknown message " + message);
                        break;
                }
            }
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"RemoteOS session caught an error with code {error}");
        }
        public async Task<ComponentList> GetComponents() { 
            if(_components == null)
            {
                _components = new(this);
                await _components.LoadAsync();
            }
            return _components;
        }
        public Computer Computer { get; }

        public async Task<string> RawExecute(string command)
        {
            string key = Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..8];
            Console.WriteLine($"Sending command: {command}");
            executequeue[key] = new(TaskCreationOptions.RunContinuationsAsynchronously);
            SendAsync(key + "\0" + command + "\r\n");
            var ret = await executequeue[key].Task;
            var err = ret[0];
            var res = string.Join("\0", ret[1..]);
            if (!string.IsNullOrWhiteSpace(err)) throw new ExecuteException(err);
            return res;
        }

        public async Task<JSONNode> Execute(string command) => JSON.Parse(await RawExecute($"return json.encode({{{command}}})"));

#if ROS_PROPERTIES
        public ComponentList Components => GetComponents().Result;
#endif
    }
}
