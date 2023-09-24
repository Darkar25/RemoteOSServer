using RemoteOS.Helpers;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("beep")]
    public class BeepComponent : Component
    {
        private const int MAX_CHANNELS = 8;

        private record FrequencyEntry(double Frequency, TimeSpan Duration, DateTime StartedAt);

        private List<FrequencyEntry> _channels = new();

        public BeepComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        public Task<int> GetBeepCount()
        {
            foreach (var expired in _channels.Where(x => x.StartedAt.Add(x.Duration) <= DateTime.Now))
                _channels.Remove(expired);
            return Task.FromResult(_channels.Count);
        }

        public async Task<bool> Beep(IDictionary<double, double> freqTable)
        {
            if (freqTable.Count > MAX_CHANNELS) throw new ArgumentOutOfRangeException(nameof(freqTable), $"Table must not contain more than {MAX_CHANNELS} frequencies");
            if (await GetBeepCount() + freqTable.Count > MAX_CHANNELS) throw new ArgumentOutOfRangeException(nameof(freqTable), $"Already too many sounds playing, maximum is {MAX_CHANNELS}");
            if (freqTable.Any(x => x.Key < 20 || x.Key > 2000)) throw new ArgumentOutOfRangeException(nameof(freqTable), "Invalid frequency, must be in [20, 2000]");
            var ret = (await GetInvoker()(freqTable))[0];
            if (ret)
                _channels.AddRange(freqTable.Select(x => new FrequencyEntry(x.Key, TimeSpan.FromMilliseconds(Math.Max(50, Math.Min(5000, (int)(x.Value * 1000)))), DateTime.Now)));
            return ret;
        }

#if ROS_PROPERTIES
        public int BeepCount => GetBeepCount().Result;
#endif
    }
}