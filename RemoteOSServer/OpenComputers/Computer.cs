using EasyJSON;
using RemoteOS.OpenComputers.Data;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace RemoteOS.OpenComputers
{
    public class Computer
    {
        Guid? _address;
        Guid? _tmpAddress;
        float? _maxEnergy;
        string? _bootaddress;
        List<string>? _users;
        IEnumerable<string>? _architectures;
        ReadOnlyDictionary<Guid, DeviceInfo>? _devices;
        string? _arch;

        public Computer(Machine parent) => Parent = parent;
        public async Task Shutdown(bool reboot = false) => await Parent.Execute($"computer.shutdown({reboot.Luaify()})");
        public async Task<(bool Success, string Reason)> AddUser(string nickname)
        {
            var res = await Parent.Execute($@"computer.addUser(""{nickname}"")");
            if (res[0]) (await GetUsers()).Add(nickname);
            return (res[0], res[1]);
        }
        public async Task<bool> RemoveUser(string nickname)
        {
            var ret = (await Parent.Execute($@"computer.removeUser(""{nickname}"")"))[0];
            if (ret) (await GetUsers()).Remove(nickname);
            return ret;
        }
        [Obsolete("Highly unrecommended, use signal events instead!")]
        public async Task PushSugnal(string name, params JSONNode[] args) => await Parent.Execute($@"computer.pushSignal(""{name}""{(args.Length > 0 ? "," + string.Join(",", args.Select(x => x.Luaify())) : "")}");

        [Obsolete("Highly unrecommended, use signal events instead!")]
        public async Task<(string Name, IEnumerable<JSONNode> Data)> PullSugnal(int timeout)
        {
            var res = await Parent.Execute($@"computer.pullSignal({timeout})");
            return (res[0], res.Linq.Skip(1).Select(x => x.Value));
        }
        public async Task Beep(string pattern)
        {
            if (!Regex.IsMatch(pattern, @"^[\.\-]+$")) throw new ArgumentException("Pattern string must contain only dots '.' and dashes '-'");
            await Parent.Execute($@"computer.beep(""{pattern}"")");
        }
        public async Task Beep(int frequency, int duration = 0)
        {
            if (frequency < 20 || frequency > 2000) throw new ArgumentOutOfRangeException(nameof(frequency), "Invalid frequency, must be in [20, 2000]");
            await Parent.Execute($@"computer.beep({frequency},{duration})");
        }

        public Machine Parent { get; private set; }
        public async Task<Guid> GetAddress() => _address ??= Guid.Parse((await Parent.Execute("computer.address()"))[0]);
        public async Task<Guid> GetTemporaryAddress() => _tmpAddress ??= Guid.Parse((await Parent.Execute("computer.tmpAddress()"))[0]);
        public async Task<int> GetFreeMemory() => (await Parent.Execute("computer.freeMemory()"))[0];
        public async Task<int> GetTotalMemory() => (await Parent.Execute("computer.totalMemory()"))[0];
        public async Task<float> GetEnergy() => (await Parent.Execute("computer.energy()"))[0];
        public async Task<float> GetMaxEnergy() => _maxEnergy ??= (await Parent.Execute("computer.maxEnergy()"))[0];
        public async Task<TimeSpan> GetUptime() => TimeSpan.FromSeconds((await Parent.Execute("computer.uptime()"))[0]);
        public async Task<string> GetBootAddress() => _bootaddress ??= (await Parent.Execute("computer.getBootAddress()"))[0];
        public async Task SetBootAddress(string address)
        {
            await Parent.Execute($@"computer.setBootAddress(""{address}"")");
            _bootaddress = address;
        }
        public async Task<string> GetRunLevel() => (await Parent.Execute("computer.runLevel()"))[0];
        public async Task<List<string>> GetUsers() => _users ??= (await Parent.Execute("computer.users()"))[0].Linq.Select(x => x.Value.Value).ToList();
        public async Task<IEnumerable<string>> GetArchitectures() => _architectures ??= (await Parent.Execute("computer.getArchitectures()"))[0].Linq.Select(x => x.Value.Value);
        public async Task<string> GetArchitecture() => _arch ??= (await Parent.Execute("computer.getArchitecture()"))[0];
        public async Task SetArchitecture(string arch)
        {
            if (!(await GetArchitectures()).Contains(arch)) throw new ArgumentException("Unknown architecture");
            await Parent.Execute($@"computer.setArchitecture(""{arch}"")"); //This returns some values but it also restarts the computer so doesnt matter
            _arch = arch;
        }
        [Obsolete("Use DateTime.Now instead")]
        public async Task<DateTime> GetRealTime() => new DateTime().AddSeconds((await Parent.Execute("computer.realTime()"))[0]);
        /// <summary>
        /// Collect information on all connected devices.
        /// </summary>
        /// <returns>Device info dictionary</returns>
        public async Task<ReadOnlyDictionary<Guid, DeviceInfo>> GetDeviceInfo()
        {
            return _devices ??= new(JSON.Parse(await Parent.RawExecute("return json.encode(computer.getDeviceInfo())")).Linq.ToDictionary(x => Guid.Parse(x.Key), x => new DeviceInfo() { 
                Class = Enum.Parse<DeviceClass>(x.Value["class"], true),
                Description = x.Value["description"],
                Vendor = x.Value["vendor"],
                Product = x.Value["product"],
                Version = x.Value["version"],
                Serial = x.Value["serial"],
                Capacity = x.Value["capacity"],
                Size = x.Value["size"],
                Clock = x.Value["clock"].Value.Split("/").Select(x => int.Parse(x)),
                Width = x.Value["width"]
            }));
        }
        public void ClearDeviceInfoCache() => _devices = null;

#if ROS_PROPERTIES
        public Guid Address => GetAddress().Result;
        public Guid TemporaryAddress => GetTemporaryAddress().Result;
        public float MaxEnergy => GetMaxEnergy().Result;
        public string BootAddress
        {
            get => GetBootAddress().Result;
            set => SetBootAddress(value);
        }
        public List<string> Users => GetUsers().Result;
        public IEnumerable<string> Architectures => GetArchitectures().Result;
        public string Architecture
        {
            get => GetArchitecture().Result;
            set => SetArchitecture(value);
        }
        public ReadOnlyDictionary<Guid, DeviceInfo> DeviceInfo => GetDeviceInfo().Result;
#if ROS_PROPS_UNCACHED
        public int FreeMemory => GetFreeMemory().Result;
        public int TotalMemory => GetTotalMemory().Result;
        public float Energy => GetEnergy().Result;
        public TimeSpan Uptime => GetUptime().Result;
        public string RunLevel => GetRunLevel().Result;
        [Obsolete("Use DateTime.Now instead")]
        public DateTime RealTime => GetRealTime().Result;
#endif
#endif
    }
}
