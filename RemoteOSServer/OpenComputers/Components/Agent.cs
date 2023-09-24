using OneOf;
using OneOf.Types;
using RemoteOS.Helpers;
using RemoteOS.OpenComputers.Exceptions;
using RemoteOS.OpenComputers.Data;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RemoteOS.OpenComputers.Components
{
    public class Agent : Component
    {
        string? _name;
        Color? _color;
        int? _selectedSlot;
        int? _selectedTank;
        int? _tnkCount;

        /// <summary>
        /// This event is sent when agent`s inventory changes
        /// <para>Parameters:
        /// <br>int - which slot changed</br>
        /// </para>
        /// </summary>
        /// <remarks>
        /// <br>Note that this only includes changes to the kind of item stored in a slot.</br>
        /// <br>For example, increasing or decreasing the size of an already present stack does not trigger this signal.</br>
        /// <br>However, swapping one item with another (say, torches with sticks) by hand will actually trigger two signals:</br>
        /// <br>one for the removal of the torches,</br>
        /// <br>one for putting the sticks into the temporarily empty slot.</br>
        /// <br>Also, this only fires for the actually addressable inventory of the robot, i.e. it does not trigger for changes in equipment (tool, card, upgrade).</br>
        /// </remarks>
        public event Action<int>? InventoryChanged;
        /// <summary>
        /// This event is sent when one of agent`s tanks changes
        /// <para>Parameters:
        /// <br>int - which tank got changed</br>
        /// <br>int - how much the tank changed</br>
        /// </para>
        /// </summary>
        public event Action<int, int>? TankChanged;

        public Agent(Machine parent, Guid address) : base(parent, address) {
            parent.Listen("inventory_changed", (parameters) => {
                InventoryChanged?.Invoke(parameters[0]);
            });
            parent.Listen("tank_changed", (parameters) => {
                TankChanged?.Invoke(parameters[0], parameters[1]);
            });
        }

        /// <summary>
        /// Perform a 'left click' towards the specified side.
        /// </summary>
        /// <param name="side">Side to perform the action on</param>
        /// <param name="sneaky">Perform action while sneaking (like when pressing the Shift key)</param>
        /// <returns>Whether the action was successful and if it was not, the reason why</returns>
        public async Task<ReasonOr<Success>> Swing(Sides side = Sides.Front, bool sneaky = false)
        {
            side.ThrowIfOtherThan(Sides.Front, Sides.Top, Sides.Bottom);
            var cmd = await GetInvoker()(side, sneaky);
            if (!cmd[0]) return cmd[1].Value;
            return new Success();
        }

        /// <inheritdoc cref="Swing(Sides, bool)"/>
        /// <param name="face">The `face' allows a more precise click calibration, and is relative to the targeted blockspace.</param>
        public async Task<ReasonOr<Success>> Swing(Sides side, Sides face, bool sneaky = false)
        {
            side.ThrowIfOtherThan(Sides.Front, Sides.Top, Sides.Bottom);
			face.ThrowIfOppositeTo(side);
			var cmd = await GetInvoker()(side, face, sneaky);
			if (!cmd[0]) return cmd[1].Value;
			return new Success();
		}

        /// <summary>
        /// Perform a 'right click' towards the specified side.
        /// </summary>
        /// <param name="side">Side to perform the action on</param>
        /// <param name="sneaky">Perform action while sneaking (like when pressing the Shift key)</param>
        /// <param name="duration">How long the agent has to perform this action</param>
        /// <returns>Whether the action was successful and if it was not, the reason why</returns>
        public async Task<ReasonOr<Success>> Use(Sides side = Sides.Front, bool sneaky = false, int duration = 0)
        {
            side.ThrowIfOtherThan(Sides.Front, Sides.Top, Sides.Bottom);
            var cmd = await GetInvoker()(side, sneaky, duration);
			if (!cmd[0]) return cmd[1].Value;
			return new Success();
		}

        /// <inheritdoc cref="Use(Sides, bool, int)"/>
        /// <param name="face">The `face' allows a more precise click calibration, and is relative to the targeted blockspace.</param>
        public async Task<ReasonOr<Success>> Use(Sides side, Sides face, bool sneaky = false, int duration = 0)
        {
            side.ThrowIfOtherThan(Sides.Front, Sides.Top, Sides.Bottom);
            face.ThrowIfOppositeTo(side);
            var cmd = await GetInvoker()(side, face, sneaky, duration);
			if (!cmd[0]) return cmd[1].Value;
			return new Success();
		}

        /// <summary>
        /// Place a block towards the specified side.
        /// </summary>
        /// <param name="side">Side to perform the action on</param>
        /// <param name="sneaky">Perform action while sneaking (like when pressing the Shift key)</param>
        /// <returns>Whether the action was successful and if it was not, the reason why</returns>
        public async Task<ReasonOr<Success>> Place(Sides side = Sides.Front, bool sneaky = false)
        {
            side.ThrowIfOtherThan(Sides.Front, Sides.Top, Sides.Bottom);
            var cmd = await GetInvoker()(side, sneaky);
			if (!cmd[0]) return cmd[1].Value;
			return new Success();
		}

        /// <inheritdoc cref="Place(Sides, bool)"/>
        /// <param name="face">The `face' allows a more precise click calibration, and is relative to the targeted blockspace.</param>
        public async Task<ReasonOr<Success>> Place(Sides side, Sides face, bool sneaky = false)
        {
            side.ThrowIfOtherThan(Sides.Front, Sides.Top, Sides.Bottom);
			face.ThrowIfOppositeTo(side);
			var cmd = await GetInvoker()(side, face, sneaky);
			if (!cmd[0]) return cmd[1].Value;
			return new Success();
		}

        /// <param name="slot">The slot to get the item count from</param>
        /// <returns>The number of items in the specified slot</returns>
        /// <exception cref="InventoryException">This slot does not exist</exception>
        public async Task<int> Count(int slot)
        {
            if (slot <= 0 || slot > await GetInventorySize()) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await GetInvoker()(slot))[0];
        }

        /// <param name="slot">The slot to get the remaining space from</param>
        /// <returns>The remaining space in the specified slot</returns>
        /// <exception cref="InventoryException">This slot does not exist</exception>
        public async Task<int> Space(int slot)
        {
            if (slot <= 0 || slot > await GetInventorySize()) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await GetInvoker()(slot))[0];
        }

        /// <summary>
        /// Compare the contents of the selected slot to the contents of the specified slot.
        /// </summary>
        /// <param name="slot">The slot to compare to</param>
        /// <param name="checkNBT">Compare the NBT data too</param>
        /// <returns>true if the items are the same, otherwise false</returns>
        /// <exception cref="InventoryException">This slot does not exist</exception>
        public async Task<bool> CompareTo(int slot, bool checkNBT = false)
        {
            if (slot <= 0 || slot > await GetInventorySize()) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await GetInvoker()(slot, checkNBT))[0];
        }

        /// <summary>
        /// Move up to the specified amount of items from the selected slot into the specified slot.
        /// </summary>
        /// <param name="slot">The slot to transfer items to</param>
        /// <returns>true if the transfer was successful, otherwise false</returns>
        /// <exception cref="InventoryException">This slot does not exist</exception>
        public async Task<bool> TransferTo(int slot)
        {
            if (slot <= 0 || slot > await GetInventorySize()) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await GetInvoker()(slot))[0];
        }

        /// <inheritdoc cref="TransferTo(int)"/>
        /// <param name="amount">How much to transfer</param>
        public async Task<bool> TransferTo(int slot, int amount)
        {
            if (slot <= 0 || slot > await GetInventorySize()) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await GetInvoker()(slot, amount));
        }

        /// <param name="slot">The tank to get level from</param>
        /// <returns>The fluid amount in the specified tank.</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<int> TankLevel(int slot)
        {
            if (slot <= 0 || slot > await GetTankCount()) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            return (await GetInvoker()(slot))[0];
        }

        /// <param name="slot">The tank to get remaining space from</param>
        /// <returns>The remaining fluid capacity in the specified tank.</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<int> TankSpace(int slot)
        {
            if (slot <= 0 || slot > await GetTankCount()) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            return (await GetInvoker()(slot))[0];
        }

        /// <summary>
        /// Compares the fluids in the selected and the specified tank.
        /// </summary>
        /// <param name="slot">The tank to compare fluid to</param>
        /// <returns>true if the fluids are equal, otherwise false</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<bool> CommareFluidTo(int slot)
        {
            if (slot <= 0 || slot > await GetTankCount()) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            return (await GetInvoker()(slot))[0];
        }

        /// <summary>
        /// Move the specified amount of fluid from the selected tank into the specified tank.
        /// </summary>
        /// <param name="slot">The tank to transfer fluid to</param>
        /// <param name="amount">How much fluid to transfer</param>
        /// <returns>true if the transfer was successful, otherwise false</returns>
        /// <exception cref="InventoryException">The destination does not exist</exception>
        public async Task<bool> TransferFluidTo(int slot, int amount = 1000)
        {
            if (slot <= 0 || slot > await GetTankCount()) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            return (await GetInvoker()(slot, amount))[0];
        }

        /// <summary>
        /// Checks the contents of the block on the specified sides and returns the findings.
        /// </summary>
        /// <param name="side">Which side to analyze</param>
        /// <returns>Whether the block is passable and its description</returns>
        public async Task<(bool Passable, string Description)> Detect(Sides side)
        {
            side.ThrowIfOtherThan(Sides.Front, Sides.Top, Sides.Bottom);
            var res = (await GetInvoker()(side))[0];
            return (!res[0], res[1]);
        }

        /// <summary>
        /// Compare the fluid in the selected tank with the fluid in the specified tank on the specified side.
        /// </summary>
        /// <param name="side">The side which has the tank</param>
        /// <returns>true is fluids are equal, otherwise false</returns>
        public async Task<bool> CompareFluid(Sides side)
        {
            side.ThrowIfOtherThan(Sides.Front, Sides.Top, Sides.Bottom);
            return (await GetInvoker()(side))[0];
        }

        /// <summary>
        /// Drains the specified amount of fluid from the specified side.
        /// </summary>
        /// <param name="side">The side to drain from</param>
        /// <param name="amount">How much to drain</param>
        /// <returns>The amount drainer or an error</returns>
        public async Task<OneOf<int, Error>> Drain(Sides side, int amount = 1000)
        {
            side.ThrowIfOtherThan(Sides.Front, Sides.Top, Sides.Bottom);
            var res = await GetInvoker()(side, amount);
			if (!res[0]) return new Error();
			return res[1].AsInt;
		}

        /// <summary>
        /// Eject the specified amount of fluid to the specified side.
        /// </summary>
        /// <param name="side">The side to fill into</param>
        /// <param name="amount">How much to fill</param>
        /// <returns>The amount filled or an error</returns>
        public async Task<OneOf<int, Error>> Fill(Sides side, int amount = 1000)
        {
            side.ThrowIfOtherThan(Sides.Front, Sides.Top, Sides.Bottom);
            var res = await GetInvoker()(side, amount);
			if (!res[0]) return new Error();
			return res[1].AsInt;
		}

        /// <summary>
        /// Compare the block on the specified side with the one in the selected slot.
        /// </summary>
        /// <param name="side">Side to compare item to</param>
        /// <param name="fuzzy">Do not compare NBT data</param>
        /// <returns>true if items are equal, otherwise false</returns>
        /// <remarks>Empty blocks are considered as air blocks, which cannot be compared to an empty inventory slot;
        /// For blocks that drop a different item, the <see cref="Compare(Sides, bool)"/> method won't work (eg: Diamond block dropping diamond items);
        /// for these cases, use silk-touch to obtain a block for comparison.</remarks>
        public async Task<bool> Compare(Sides side, bool fuzzy = false)
        {
            side.ThrowIfOtherThan(Sides.Front, Sides.Top, Sides.Bottom);
            return (await GetInvoker()(side, fuzzy))[0];
        }

        /// <summary>
        /// Drops items from the selected slot towards the specified side.
        /// </summary>
        /// <param name="side">The side to drop items from</param>
        /// <param name="amount">How much items to drop</param>
        /// <returns>true if items were dropped succesfully, otherwise false</returns>
        public async Task<bool> Drop(Sides side, int amount = 64)
        {
            side.ThrowIfOtherThan(Sides.Front, Sides.Top, Sides.Bottom);
            return (await GetInvoker()(side, amount))[0];
        }

        /// <summary>
        /// Suck up items from the specified side.
        /// </summary>
        /// <param name="side">The to suck from</param>
        /// <param name="amount">How many items to suck</param>
        /// <returns>The amount sucked or an error</returns>
        public async Task<OneOf<int, Error>> Suck(Sides side, int amount = 64)
        {
            side.ThrowIfOtherThan(Sides.Front, Sides.Top, Sides.Bottom);
            var res = (await GetInvoker()(side, amount))[0];
			if (res[0].IsBoolean) return new Error();
			return res[0].AsInt;
        }

        /// <returns>The name of agent.</returns>
        public async Task<string> GetName() => _name ??= (await Invoke("name"))[0];

        /// <returns>The current color of the activity light</returns>
        public async Task<Color> GetLightColor() => _color ??= Color.FromArgb((await GetInvoker()())[0]);

        /// <summary>
        /// Set the color of the activity light to the specified color
        /// </summary>
        /// <param name="color">New color</param>
        public async Task SetLightColor(Color color) => _color = Color.FromArgb((await GetInvoker()(color))[0]);

        /// <returns>The size of this device's internal inventory.</returns>
        public async Task<int> GetInventorySize() => (await this.GetDeviceInfo()).Capacity;

        /// <returns>The currently selected slot</returns>
        public async Task<int> GetSelectedSlot() => _selectedSlot ??= (await Invoke("select"))[0];

        /// <summary>
        /// Set the selected slot
        /// </summary>
        /// <param name="slot">Which slot to select</param>
        /// <exception cref="InventoryException">This slot does not exist</exception>
        public async Task SetSelectedSlot(int slot)
        {
            if (slot <= 0 || slot > await GetInventorySize()) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            if (slot == await GetSelectedSlot()) return; //Nothing to do
            _selectedSlot = (await Invoke("select", slot))[0];
        }

        /// <returns>The number of items in the selected slot</returns>
        public async Task<int> GetSelectedSlotCount() => (await Invoke("count"))[0];

        /// <returns>The remaining space in the selected slot</returns>
        public async Task<int> GetSelectedSlotSpace() => (await Invoke("space"))[0];

        /// <returns>The number of tanks installed in the device.</returns>
        public async Task<int> GetTankCount() => _tnkCount ??= (await Invoke("tankCount"))[0];

        /// <returns>The number of the currently selected tank.</returns>
        public async Task<int> GetSelectedTank() => _selectedTank ??= (await Invoke("selectTank"))[0];

        /// <summary>
        /// Select a tank and get the number of the currently selected tank.
        /// </summary>
        /// <param name="tank">Which tank to select</param>
        /// <returns>Selected tank</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<int> SetSelectedTank(int tank)
        {
            if (tank <= 0 || tank > await GetTankCount()) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            if (tank != await GetSelectedTank()) //Check if we really need to change the tank
                _selectedTank = (await Invoke("selectTank", tank))[0];
            return await GetSelectedTank();
        }

        /// <returns>The fluid amount in the selected tank.</returns>
        public async Task<int> GetSelectedTankLevel() => (await Invoke("tankLevel"))[0];

        /// <returns>The remaining space in the selected tank.</returns>
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
