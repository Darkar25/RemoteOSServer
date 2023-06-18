using EasyJSON;
using NetCoreServer;
using OneOf.Types;
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
        readonly Dictionary<string, Action<JSONArray>> SignalEvnets = new();
        ComponentList? _components;

		// To prevent race condition
		TaskCompletionSource<ComponentList>? cl_lock;

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
            foreach (var t in executequeue.Values)
                t.SetCanceled();
        }

        /// <summary>
        /// Listen for incoming signals of specified type
        /// </summary>
        /// <param name="signal">Signal type</param>
        /// <param name="action">Handler</param>
        public void Listen(string signal, Action<JSONArray> action)
        {
            if(SignalEvnets.ContainsKey(signal)) SignalEvnets[signal] += action;
            else SignalEvnets[signal] = action;
        }

        /// <summary>
        /// Push the signal to all available listeners
        /// </summary>
        /// <param name="signal">Signal type</param>
        /// <param name="parameters">Signal parameters</param>
        public void FireSignal(string signal, params JSONNode[] parameters)
        {
            if (SignalEvnets.TryGetValue(signal, out var act))
                act(new(parameters));
        }

		// Perhaps change the scheme from
		//      (e|r)\x0.*
		// to
		//      {"type": "signal", "data": ["some", "data", "here"]}
		//      {"type": "success", "data": ["some", "data", "here"], "key": "asdf1234"}
		//      {"type": "error", "data": ["error text here"], "key": "asdf1234"}
		// this hurts performance but increases flexibility and code readability
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
                            task.SetResult(a[2..]);
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

        /// <returns>The component list of this machine</returns>
        public async Task<ComponentList> GetComponents() {
            if (_components is not null) return _components;
            if (cl_lock is not null) return await cl_lock.Task;
            cl_lock = new();
            _components = await new ComponentList(this).LoadAsync();
            cl_lock.SetResult(_components);
			return _components;
        }

        /// <summary>
        /// The computer associated with this machine
        /// </summary>
        public Computer Computer { get; }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <returns>Result of the command</returns>
        /// <exception cref="ExecuteException">Remote machine threw an error while executing the command</exception>
        public async Task<string> RawExecute(string command)
        {
            string key = Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..8];
            Console.WriteLine($"Sending command: {command}");
            executequeue[key] = new(TaskCreationOptions.RunContinuationsAsynchronously);
            SendAsync(key + "\0" + command + "\r\n");
            var ret = await executequeue[key].Task;
			executequeue.Remove(key, out _);
			var err = ret[0];
            var res = string.Join("\0", ret[1..]);
            if (!string.IsNullOrWhiteSpace(err)) throw new ExecuteException(err);
            return res;
        }

        /// <summary>
        /// Executes the command and wraps its result in json
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <returns>Result of the command in JSON format</returns>
        public async Task<JSONNode> Execute(string command) => JSON.Parse(await RawExecute($"return json.encode({{{command}}})"));

#if ROS_PROPERTIES
        public ComponentList Components => GetComponents().Result;
#endif
    }
}
