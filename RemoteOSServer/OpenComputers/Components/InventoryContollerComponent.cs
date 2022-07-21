using RemoteOS.OpenComputers.Data;
using RemoteOS.OpenComputers.Exceptions;

namespace RemoteOS.OpenComputers.Components
{
    [Component("inventory_controller")]
    public class InventoryContollerComponent : Component
    {
        public InventoryContollerComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        public async Task<int> GetInventorySize(Sides side)
        {
            if (side != Sides.Front || side != Sides.Top || side != Sides.Bottom) throw new InvalidSideException("Valid sides are Front, Top, Bottom");
            return (await Invoke("getInventorySize", side))[0];
        }

        public async Task<ItemStackInfo> GetStackInSlot(Sides side, int slot)
        {
            if (side != Sides.Front || side != Sides.Top || side != Sides.Bottom) throw new InvalidSideException("Valid sides are Front, Top, Bottom");
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            var res = (await Invoke("getStackInSlot", side, slot))[0];
            return new(res);
        }

        public async Task<ItemStackInfo> GetStackInInternalSlot(int slot)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            var res = (await Invoke("getStackInInternalSlot", slot))[0];
            return new(res);
        }

        public async Task<ItemStackInfo> GetStackInInternalSlot()
        {
            var res = (await Invoke("getStackInInternalSlot"))[0];
            return new(res);
        }

        public async Task<(bool Success, string Reason)> DropIntoSlot(Sides side, int slot, int count = 64) => await DropIntoSlot(side, slot, count, side);

        public async Task<(bool Success, string Reason)> DropIntoSlot(Sides face, int slot, int count, Sides side)
        {
            if (side != Sides.Front || side != Sides.Top || side != Sides.Bottom) throw new InvalidSideException("Valid sides are Front, Top, Bottom");
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            var res = await Invoke("dropIntoSlot", face, slot, count, side);
            return (res[0], res[1]);
        }

        public async Task<(bool Success, string Reason)> SuckFromSlot(Sides side, int slot, int count = 64) => await SuckFromSlot(side, slot, count, side);

        public async Task<(bool Success, string Reason)> SuckFromSlot(Sides face, int slot, int count, Sides side)
        {
            if (side != Sides.Front || side != Sides.Top || side != Sides.Bottom) throw new InvalidSideException("Valid sides are Front, Top, Bottom");
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            var res = await Invoke("suckFromSlot", face, slot, count, side);
            return (res[0], res[1]);
        }

        public async Task<bool> Equip() => (await Invoke("equip"))[0];

        public async Task<bool> Store(Sides side, int slot, DatabaseComponent database, int dbSlot)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("store", side, slot, database.Address, dbSlot))[0];
        }

        public async Task<bool> StoreInternal(int slot, DatabaseComponent database, int dbSlot)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("storeInternal", slot, database.Address, dbSlot))[0];
        }

        public async Task<bool> CompareToDatabase(int slot, DatabaseComponent database, int dbSlot, bool checkNBT = false)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("compareToDatabase", slot, database.Address, dbSlot, checkNBT))[0];
        }

        public async Task<bool> CompareStacks(Sides side, int slotA, int slotB, bool checkNBT = false)
        {
            if (slotA <= 0 || slotB <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("compareStacks", side, slotA, slotB, checkNBT))[0];
        }

        public async Task<int> GetSlotMaxStackSize(Sides side, int slot)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("getSlotMaxStackSize", side, slot))[0];
        }

        public async Task<int> GetSlotStackSize(Sides side, int slot)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("getSlotStackSize", side, slot))[0];
        }
    }
}
