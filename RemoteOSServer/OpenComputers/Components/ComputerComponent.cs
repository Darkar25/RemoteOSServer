﻿using RemoteOS.OpenComputers.Data;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace RemoteOS.OpenComputers.Components
{
    [Component("computer")]
    public class ComputerComponent : Component
    {
        public ComputerComponent(Machine parent, Guid address) : base(parent, address) { }
        /// <summary>
        /// Starts the computer.
        /// </summary>
        /// <returns>true is the state changed</returns>
        public async Task<bool> Start() => (await Invoke("start"))[0];
        /// <summary>
        /// Stops the computer.
        /// </summary>
        /// <returns>true is the state changed</returns>
        public async Task<bool> Stop() => (await Invoke("stop"))[0];
        /// <summary>
        /// Plays a tone, useful to alert users via audible feedback.
        /// </summary>
        /// <param name="frequency">Frequency to beep on</param>
        /// <param name="duration">How long to beep for</param>
        public async Task Beep(int frequency, int duration = 0)
        {
            if (frequency < 20 || frequency > 2000) throw new ArgumentOutOfRangeException(nameof(frequency), "Invalid frequency, must be in [20, 2000]");
            await Invoke("beep", frequency, duration);
        }
        /// <summary>
        /// Plays a tone, useful to alert users via audible feedback.
        /// </summary>
        /// <param name="pattern">Dot and dash pattern for computer to beep</param>
        public async Task Beep(string pattern)
        {
            if (!Regex.IsMatch(pattern, @"^[\.\-]+$")) throw new ArgumentException("Pattern string must contain only dots '.' and dashes '-'"); 
            await Invoke("beep", pattern);
        }
        /// <returns>Whether the computer is running.</returns>
        public async Task<bool> IsRunning() => (await Invoke("isRunning"))[0];
#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public bool Running => IsRunning().Result;
#endif
    }
}
