using EasyJSON;
using OneOf;
using OneOf.Types;
using RemoteOS.OpenComputers.Data;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using RemoteOS.Helpers;

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

        public Machine Parent { get; private set; }

        public Computer(Machine parent) => Parent = parent;

        /// <summary>
        /// Shutdown or reboot the computer.
        /// </summary>
        /// <param name="reboot">Should reboot the computer after shutting down</param>
        public async Task Shutdown(bool reboot = false) => await Parent.Execute($"computer.shutdown({reboot.Luaify()})");

        /// <summary>
        /// Add a player to the machine's list of users, by username. This requires for the player to be online.
        /// </summary>
        /// <param name="nickname">the name of the player to add as a user.</param>
        /// <returns>Whether the player was added to the user list.</returns>
        public async Task<ReasonOr<Success>> AddUser(string nickname)
        {
            var users = await GetUsers();
            if (users.Contains(nickname)) return new Success();
			var res = await Parent.Execute($@"computer.addUser({nickname.Luaify()})");
            if (!res[0]) return res[1].Value;
			users.Add(nickname);
			return new Success();
		}

        /// <summary>
        /// Removes a player as a user from this machine, by username. Unlike when adding players, the player does not have to be online to be removed from the list.
        /// </summary>
        /// <param name="nickname">the name of the player to remove.</param>
        /// <returns>whether the player was removed from the user list.</returns>
        public async Task<bool> RemoveUser(string nickname)
        {
			var users = await GetUsers();
            if (!users.Contains(nickname)) return true;
			var ret = await Parent.Execute($@"computer.removeUser({nickname.Luaify()})");
			if (ret[0]) users.Remove(nickname);
            return ret[0];
        }

        [Obsolete("Highly unrecommended, use signal events instead!")]
        public async Task PushSugnal(string name, params JSONNode[] args) => await Parent.Execute($@"computer.pushSignal({name.Luaify()}{(args.Length > 0 ? "," + string.Join(",", args.Select(x => x.Luaify())) : "")}");

        [Obsolete("Highly unrecommended, use signal events instead!")]
        public async Task<(string Name, IEnumerable<JSONNode> Data)> PullSugnal(int timeout)
        {
            var res = await Parent.Execute($@"computer.pullSignal({timeout})");
            return (res[0], res.Linq.Skip(1).Select(x => x.Value));
        }

        /// <summary>
        /// Utility method for playing beep codes. 
        /// The underlying functionality is similar to that of <see cref="Beep(int, int)"/>,
        /// except that this will play tones at a fixed frequency, and two different durations - in a pattern as defined in the passed string.
        /// This is useful for generating beep codes, such as for boot errors.
        /// </summary>
        /// <param name="pattern">pattern the beep pattern to play.</param>
        /// <exception cref="ArgumentException">String pattern is invalid</exception>
        public async Task Beep(string pattern)
        {
            if (!Regex.IsMatch(pattern, @"^[\.\-]+$")) throw new ArgumentException("Pattern string must contain only dots '.' and dashes '-'", nameof(pattern));
            await Parent.Execute($@"computer.beep({pattern.Luaify()})");
        }

        /// <summary>
        /// Play a sound using the machine's built-in speaker.
        /// </summary>
        /// <param name="frequency">the frequency of the tone to generate.</param>
        /// <param name="duration">the duration of the tone to generate, in milliseconds.</param>
        /// <exception cref="ArgumentOutOfRangeException">Frequency is invalid</exception>
        public async Task Beep(int frequency, int duration = 0)
        {
            if (frequency < 20 || frequency > 2000) throw new ArgumentOutOfRangeException(nameof(frequency), "Invalid frequency, must be in [20, 2000]");
            await Parent.Execute($@"computer.beep({frequency},{duration})");
        }

        /// <returns>The address of this computer</returns>
        public async Task<Guid> GetAddress() => _address ??= Guid.Parse((await Parent.Execute("computer.address()"))[0]);

        /// <returns>The temporary address</returns>
        public async Task<Guid> GetTemporaryAddress() => _tmpAddress ??= Guid.Parse((await Parent.Execute("computer.tmpAddress()"))[0]);

        /// <returns>Remaining memory</returns>
        public async Task<int> GetFreeMemory() => (await Parent.Execute("computer.freeMemory()"))[0];

        /// <returns>Total memory</returns>
        public async Task<int> GetTotalMemory() => (await Parent.Execute("computer.totalMemory()"))[0];

        /// <returns>Remaining energy</returns>
        public async Task<float> GetEnergy() => (await Parent.Execute("computer.energy()"))[0];

        /// <returns>Total energy</returns>
        public async Task<float> GetMaxEnergy() => _maxEnergy ??= (await Parent.Execute("computer.maxEnergy()"))[0];

        /// <returns>How long the computer was running for</returns>
        public async Task<TimeSpan> GetUptime() => TimeSpan.FromSeconds((await Parent.Execute("computer.uptime()"))[0]);

        /// <returns>The address that the computer is booting from</returns>
        public async Task<string> GetBootAddress() => _bootaddress ??= (await Parent.Execute("computer.getBootAddress()"))[0];

        /// <summary>
        /// Sets the boot address
        /// </summary>
        /// <param name="address">New boot address</param>
        public async Task SetBootAddress(string address)
        {
            await Parent.Execute($@"computer.setBootAddress({address.Luaify()})");
            _bootaddress = address;
        }

        public async Task<string> GetRunLevel() => (await Parent.Execute("computer.runLevel()"))[0];

        /// <returns>The list of users registered on this machine.</returns>
        public async Task<List<string>> GetUsers() => _users ??= (await Parent.Execute("computer.users()"))[0].Linq.Select(x => x.Value.Value).ToList();

        public async Task<IEnumerable<string>> GetArchitectures() => _architectures ??= (await Parent.Execute("table.unpack(computer.getArchitectures())")).Linq.Select(x => x.Value.Value);
        
        /// <summary>
        /// This is what actually evaluates code running on the machine, where the machine class itself serves as a scheduler.
        /// </summary>
        /// <returns>The underlying architecture of the machine.</returns>
        public async Task<string> GetArchitecture() => _arch ??= (await Parent.Execute("computer.getArchitecture()"))[0];
        
        /// <summary>
        /// Sets the architecture for this computer
        /// </summary>
        /// <param name="arch">New architecture</param>
        /// <exception cref="ArgumentException">This architecture does not exist</exception>
        public async Task SetArchitecture(string arch)
        {
            if (!(await GetArchitectures()).Contains(arch)) throw new ArgumentException("Unknown architecture");
            await Parent.Execute($@"computer.setArchitecture({arch.Luaify()})"); //This returns some values but it also restarts the computer so doesnt matter
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
            return _devices ??= new(JSON.Parse((await Parent.RawExecute("return json.encode(computer.getDeviceInfo())"))).Linq.ToDictionary(x => Guid.Parse(x.Key), x => new DeviceInfo() { 
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
        
        /// <summary>
        /// Clears the device info cache, so the next <see cref="GetDeviceInfo"/> call will be forced to load the device info from the remote machine.
        /// </summary>
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
