using RemoteOS.Helpers;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("self_destruct"), Component("server_destruct")]
    public partial class BoomComponent : Component
    {
        private DateTime? _triggerTime;

        public BoomComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Starts the countdown; Will be ticking down until the time is reached. 5 seconds by default.
        /// </summary>
        /// <param name="fuse">The time before explosion in seconds</param>
        /// <returns>The time set</returns>
        /// <exception cref="InvalidOperationException">Fuse has already been set</exception>
        /// <exception cref="ArgumentOutOfRangeException">Fuse is too long</exception>
        public async Task<int> Start(double fuse = 5)
        {
            if (_triggerTime.HasValue) throw new InvalidOperationException("Fuse has already been set");
            if (fuse > 100_000) throw new ArgumentOutOfRangeException(nameof(fuse), "Time may not be greater than 100000");
            var ret = (await GetInvoker()(fuse))[0];
            _triggerTime = DateTime.Now.AddSeconds(ret);
            return ret;
        }

        /// <returns>The time in seconds left or -1 if the fuse has not been set yet</returns>
        public async Task<double> Time()
        {
            if (!_triggerTime.HasValue)
            {
                var time = (await GetInvoker()())[0];
                if (time < 0) return -1;
                _triggerTime = DateTime.Now.AddSeconds(time);
            }
            return (_triggerTime - DateTime.Now).Value.TotalSeconds;
        }

#if ROS_PROPERTIES
        public double Fuse
        {
            get => Time().Result;
            set => Start(value);
        }
#endif
    }
}