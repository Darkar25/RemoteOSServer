using RemoteOS.OpenComputers.Data;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("computer")]
    public partial class ComputerComponent : Component
    {
        Tier? _tier;

        public ComputerComponent(Machine parent, Guid address) : base(parent, address) {
            Parent.Listen("component_added", (parameters) => {
                if (_tier == Tier.Three) _tier = null;
            });
        }

        public override async Task<Tier> GetTier()
        {
            if (_tier is not null) return _tier.Value;
            var s = (await this.GetDeviceInfo()).Capacity;
            if (s == 7) _tier = Tier.One;
            else if (s == 8) _tier = Tier.Two;
            else
            {
                _tier = Tier.Three;
                if (Address == await Parent.Computer.GetAddress())
                {
                    var threeCount = 0;
                    foreach (var c in (await Parent.GetComponents()).Where((x) => x.GetSlot().Result != -1))
                    {
                        if (c != this && await c.GetTier() >= Tier.Three) threeCount++;
                        if (threeCount > 1)
                        {
                            _tier = Tier.Four;
                            break;
                        }
                    }
                }
            }
            return _tier.Value;
        }

        /// <summary>
        /// Starts the computer.
        /// </summary>
        /// <returns>true is the state changed</returns>
        public partial Task<bool> Start();

        /// <summary>
        /// Stops the computer.
        /// </summary>
        /// <returns>true is the state changed</returns>
        public partial Task<bool> Stop();

        /// <summary>
        /// Plays a tone, useful to alert users via audible feedback.
        /// </summary>
        /// <param name="frequency">Frequency to beep on</param>
        /// <param name="duration">How long to beep for</param>
        public Task Beep(int frequency, int duration = 0)
        {
            if (frequency < 20 || frequency > 2000) throw new ArgumentOutOfRangeException(nameof(frequency), "Invalid frequency, must be in [20, 2000]");
            return Invoke("beep", frequency, duration);
        }

        /// <summary>
        /// Plays a tone, useful to alert users via audible feedback.
        /// </summary>
        /// <param name="pattern">Dot and dash pattern for computer to beep</param>
        public Task Beep(string pattern)
        {
            if (!Regex.IsMatch(pattern, @"^[\.\-]+$")) throw new ArgumentException("Pattern string must contain only dots '.' and dashes '-'"); 
            return Invoke("beep", pattern);
        }

        /// <returns>Whether the computer is running.</returns>
        public partial Task<bool> IsRunning();

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public bool Running => IsRunning().Result;
#endif
    }
}
