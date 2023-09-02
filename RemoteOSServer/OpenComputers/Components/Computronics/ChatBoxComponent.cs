using System.Runtime.InteropServices;
using RemoteOS.Helpers;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("chat_box")]
    public class ChatBoxComponent : Component
    {
        public ChatBoxComponent(Machine parent, Guid address) : base(parent, address)
        {
            parent.Listen("chat_message", (parameters) =>
            {
                if (parameters[0] == Address.ToString())
                {
                    ChatMessage?.Invoke((Guid) Guid.Parse(parameters[0]), (string) parameters[1],
                        (string) parameters[2]);
                }
            });
        }

        /// <summary>
        /// Event triggered when chatbox catch message in the radius of actions
        /// </summary>
        public event Action<Guid, string, string?>? ChatMessage;

        /// <summary>
        /// Get the current distance at which messages will be intercepted
        /// </summary>
        /// <returns>Distance</returns>
        public async Task<int> GetDistance() => (await Invoke("getDistance"))[0];

        /// <summary>
        /// Get current name of chatbox
        /// </summary>
        /// <returns>Chatbox name</returns>
        public async Task<string> GetName() => (await Invoke("getName"))[0];

        /// <summary>
        /// Execute method "say", which send message in chat 
        /// </summary>
        /// <param name="text">Text of message</param>
        /// <param name="distance">Distance, to which message will be sended</param>
        /// <returns>True if success</returns>
        public async Task<bool> Say(string text, [Optional] int distance) => (await Invoke("say", text, distance))[0];

        /// <summary>
        /// Set distance on which chatbox will work
        /// </summary>
        /// <param name="distance">Distance</param>
        /// <returns>Current dist</returns>
        public async Task<int> SetDistance(int distance) => (await Invoke("setDistance", distance))[0];

        /// <summary>
        /// Set chatbox name. It will be displayed before the messages sent by the chatbox. Like at player.
        /// </summary>
        /// <param name="name">Chatbox name</param>
        /// <returns>Current Name</returns>
        public async Task<string> SetName(string name) => (await Invoke("setName", name))[0];

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public string Name
        {
            get => GetName().Result;
            set => SetName(value);
        }

        public int Distance
        {
            get => GetDistance().Result;
            set => SetDistance(value);
        }
#endif
    }
}