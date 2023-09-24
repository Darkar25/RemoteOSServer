using System.Runtime.InteropServices;
using RemoteOS.Helpers;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("chat_box"), Component("chat")]
    public partial class ChatBoxComponent : Component
    {
        public ChatBoxComponent(Machine parent, Guid address) : base(parent, address)
        {
            parent.Listen("chat_message", (parameters) =>
            {
                if (parameters[0] == Address.ToString())
                {
                    ChatMessage?.Invoke((string) parameters[1], (string) parameters[2]);
                }
            });
        }

        /// <summary>
        /// Event triggered when chatbox catch message in the radius of actions
        /// </summary>
        public event Action<string, string?>? ChatMessage;

        /// <summary>
        /// Get the current distance at which messages will be intercepted
        /// </summary>
        /// <returns>Distance</returns>
        public partial Task<int> GetDistance();

        /// <summary>
        /// Get current name of chatbox
        /// </summary>
        /// <returns>Chatbox name</returns>
        public partial Task<string> GetName();

        /// <summary>
        /// Execute method "say", which send message in chat 
        /// </summary>
        /// <param name="text">Text of message</param>
        /// <param name="distance">Distance, to which message will be sended</param>
        /// <returns>True if success</returns>
        public partial Task<bool> Say(string text, [Optional] int distance);

        /// <summary>
        /// Set distance on which chatbox will work
        /// </summary>
        /// <param name="distance">Distance</param>
        /// <returns>Current dist</returns>
        public partial Task<int> SetDistance(int distance);

        /// <summary>
        /// Set chatbox name. It will be displayed before the messages sent by the chatbox. Like at player.
        /// </summary>
        /// <param name="name">Chatbox name</param>
        /// <returns>Current Name</returns>
        public partial Task<string> SetName(string name);

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