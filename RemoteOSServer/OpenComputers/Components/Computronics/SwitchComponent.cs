using RemoteOS.Helpers;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("switch_board")]
    public partial class SwitchComponent : Component
    {
        private bool?[] _switches = new bool?[4];

        /// <summary>
        /// This event is sent when one of the switches get flipped
        /// <para>Parameters:
        /// <br>int - which switch flipped</br>
        /// <br>bool - the new state of the switch</br>
        /// </para>
        /// </summary>
        public event Action<int, bool>? SwitchFlipped;

        public SwitchComponent(Machine parent, Guid address) : base(parent, address)
        {
            parent.Listen("switch_flipped", (parameters) =>
            {
                if (parameters[0] == Address.ToString())
                {
                    _switches[parameters[1]] = parameters[2];
                    SwitchFlipped?.Invoke(parameters[1], parameters[2]);
                }
            });
        }

        /// <summary>
        /// Activates or deactivates the specified switch.
        /// </summary>
        /// <param name="index">Switch index</param>
        /// <param name="state">The new state</param>
        /// <returns>True if the state changed.</returns>
        /// <exception cref="IndexOutOfRangeException">This switch does not exist</exception>
        public async Task<bool> SetActive(int index, bool state)
        {
            if (index < 0 || index >= _switches.Length) throw new IndexOutOfRangeException("This switch does not exist");
            var ret = (await GetInvoker()(index, state))[0];
            if (ret) _switches[index] = state;
            return ret;
        }

        /// <param name="index">Switch index</param>
        /// <returns>True if the switch at the specified position is currently active</returns>
        /// <exception cref="IndexOutOfRangeException">This switch does not exist</exception>
        public async Task<bool> IsActive(int index)
        {
            if (index < 0 || index >= _switches.Length) throw new IndexOutOfRangeException("This switch does not exist");
            return _switches[index] ??= (await GetInvoker()(index))[0];
        }

#if ROS_PROPERTIES
        public bool this[int index]
        {
            get => IsActive(index).Result;
            set => SetActive(index, value);
        }
#endif
    }
}