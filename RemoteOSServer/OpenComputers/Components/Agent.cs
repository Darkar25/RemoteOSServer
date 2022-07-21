using RemoteOS.OpenComputers.Exceptions;
using System.Drawing;

namespace RemoteOS.OpenComputers.Components
{
    public class Agent : Component
    {
        string? _name;
        Color? _color;
        int? _selectedSlot;
        int? _selectedTank;
        int? _invSize;
        int? _tnkCount;

        public event Action<int>? InventoryChanged;
        public event Action<int, int>? TankChanged;

        public Agent(Machine parent, Guid address) : base(parent, address) {
            parent.Listen("inventory_changed", (parameters) => {
                InventoryChanged?.Invoke(parameters[0]);
            });
            parent.Listen("tank_changed", (parameters) => {
                TankChanged?.Invoke(parameters[0], parameters[1]);
            });
        }

        public async Task<(bool Success, string Reason)> Swing(Sides side = Sides.Front, bool sneaky = false) => await Swing(side, side, sneaky);
        public async Task<(bool Success, string Reason)> Swing(Sides side, Sides face, bool sneaky = false)
        {
            var cmd = await Invoke("swing", side, face, sneaky);
            return (cmd[0], cmd[1]);
        }
        public async Task<(bool Success, string Reason)> Use(Sides side = Sides.Front, bool sneaky = false, int duration = 0) => await Use(side, side, sneaky, duration);
        public async Task<(bool Success, string Reason)> Use(Sides side, Sides face, bool sneaky = false, int duration = 0)
        {
            var cmd = await Invoke("use", side, face, sneaky, duration);
            return (cmd[0], cmd[1]);
        }
        public async Task<(bool Success, string Reason)> Place(Sides side = Sides.Front, bool sneaky = false) => await Place(side, side, sneaky);
        public async Task<(bool Success, string Reason)> Place(Sides side, Sides face, bool sneaky = false)
        {
            var cmd = await Invoke("place", side, face, sneaky);
            return (cmd[0], cmd[1]);
        }
        public async Task<int> Count(int slot)
        {
            if (slot <= 0 || slot > await GetInventorySize()) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("count", slot))[0];
        }
        public async Task<int> Space(int slot)
        {
            if (slot <= 0 || slot > await GetInventorySize()) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("space", slot))[0];
        }
        public async Task<bool> CompareTo(int slot)
        {
            if (slot <= 0 || slot > await GetInventorySize()) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("compareTo", slot))[0];
        }
        public async Task<bool> TransferTo(int slot)
        {
            if (slot <= 0 || slot > await GetInventorySize()) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("transferTo", slot))[0];
        }
        public async Task<bool> TransferTo(int slot, int amount)
        {
            if (slot <= 0 || slot > await GetInventorySize()) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("transferTo", slot, amount))[0];
        }
        public async Task<int> TankLevel(int slot)
        {
            if (slot <= 0 || slot > await GetTankCount()) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            return (await Invoke("tankLevel", slot))[0];
        }
        public async Task<int> TankSpace(int slot)
        {
            if (slot <= 0 || slot > await GetTankCount()) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            return (await Invoke("tankSpace", slot))[0];
        }
        public async Task<bool> CommareFluidTo(int slot)
        {
            if (slot <= 0 || slot > await GetTankCount()) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            return (await Invoke("compareFluidTo", slot))[0];
        }
        public async Task<bool> TransferFluidTo(int slot, int amount = 1000)
        {
            if (slot <= 0 || slot > await GetTankCount()) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            return (await Invoke("transferFluidTo", slot, amount))[0];
        }
        public async Task<(bool Passable, string Description)> Detect(Sides side)
        {
            var res = await Invoke("detect", side);
            return (!res[0], res[1]);
        }
        public async Task<bool> CompareFluid(Sides side) => (await Invoke("compareFluid", side))[0];
        public async Task<bool> Drain(Sides side, int amount = 1000) => (await Invoke("drain", side, amount))[0];
        public async Task<bool> Fill(Sides side, int amount = 1000) => (await Invoke("fill", side, amount))[0];
        public async Task<bool> Compare(Sides side, bool fuzzy = false) => (await Invoke("compare", side, fuzzy))[0];
        public async Task<bool> Drop(Sides side, int amount = 64) => (await Invoke("drop", side, amount))[0];
        public async Task<(bool Success, int Amount)> Suck(Sides side, int amount = 64)
        {
            var res = await Invoke("suck", side, amount);
            var f = res[0].IsBoolean;
            return (!f, f ? 0 : res[0]);
        }

        public async Task<string> GetName() => _name ??= (await Invoke("name"))[0];
        public async Task<Color> GetLightColor() => _color ??= Color.FromArgb((await Invoke("getLightColor"))[0]);
        public async Task SetLightColor(Color color) => _color = Color.FromArgb((await Invoke("setLightColor", color.ToArgb()))[0]);
        public async Task<int> GetInventorySize() => _invSize ??= (await Invoke("inventorySize"))[0];
        public async Task<int> GetSelectedSlot() => _selectedSlot ??= (await Invoke("select"))[0];
        public async Task SetSelectedSlot(int slot)
        {
            if (slot > await GetInventorySize() || slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            _selectedSlot = (await Invoke("select", slot))[0];
        }
        public async Task<int> GetSelectedSlotCount() => (await Invoke("count"))[0];
        public async Task<int> GetSelectedSlotSpace() => (await Invoke("space"))[0];
        public async Task<int> GetTankCount() => _tnkCount ??= (await Invoke("tankCount"))[0];
        public async Task<int> GetSelectedTank() => _selectedTank ??= (await Invoke("selectTank"))[0];
        public async Task SetSelectedTank(int tank)
        {
            if (tank > await GetTankCount() || tank <= 0) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            _selectedTank = (await Invoke("selectTank", tank))[0];
        }
        public async Task<int> GetSelectedTankLevel() => (await Invoke("tankLevel"))[0];
        public async Task<int> GetSelectedTankSpace() => (await Invoke("tankSpace"))[0];

#if ROS_PROPERTIES
        public string Name => GetName().Result;
        public Color Color
        {
            get => GetLightColor().Result;
            set => SetLightColor(value);
        }
        public int InventorySize => GetInventorySize().Result;
        public int SelectedSlot
        {
            get => GetSelectedSlot().Result;
            set => SetSelectedSlot(value);
        }
        public int TankCount => GetTankCount().Result;
        public int SelectedTank
        {
            get => GetSelectedTank().Result;
            set => SetSelectedTank(value);
        }
#if ROS_PROPS_UNCACHED
        public int SelectedSlotCount => GetSelectedSlotCount().Result;
        public int SelectedSlotSpace => GetSelectedSlotSpace().Result;
        public int SelectedTankLevel => GetSelectedTankLevel().Result;
        public int SelectedTankSpace => GetSelectedTankSpace().Result;
#endif
#endif
    }
}
