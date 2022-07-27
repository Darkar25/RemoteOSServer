using RemoteOS.OpenComputers.Data;
using RemoteOS.OpenComputers.Exceptions;

namespace RemoteOS.OpenComputers.Components
{
    [Obsolete("Store and compare stack data on the server, not on the remote machine.")]
    [Component("database")]
    public class DatabaseComponent : Component
    {
        public DatabaseComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        public ItemStackInfo this[int slot] => Get(slot).Result;

        /// <param name="slot">The slot to get item from</param>
        /// <returns>Representation of the item stack stored in the specified slot.</returns>
        /// <exception cref="InventoryException">This slot does not exist</exception>
        public async Task<ItemStackInfo> Get(int slot)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return new ItemStackInfo((await Invoke("get", slot))[0]);
        }
        /// <summary>
        /// Computes a hash value for the item stack in the specified slot.
        /// </summary>
        /// <param name="slot">Slot to compute hash for</param>
        /// <returns>Hash string</returns>
        /// <exception cref="InventoryException">This slot does not exist</exception>
        public async Task<string> ComputeHash(int slot)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("computeHash", slot))[0];
        }
        /// <summary>
        /// Get the index of an item stack with the specified hash.
        /// </summary>
        /// <param name="hash">Hash of an item</param>
        /// <returns>The index of an item, a negative value if no such stack was found.</returns>
        public async Task<int> IndexOf(string hash) => (await Invoke("indexOf", hash))[0];
        /// <summary>
        /// Clears the specified slot.
        /// </summary>
        /// <param name="slot">Slot to clear</param>
        /// <returns>true if there was something in the slot before.</returns>
        /// <exception cref="InventoryException">This slot does not exist</exception>
        public async Task<bool> CLear(int slot)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("clear", slot))[0];
        }
        /// <summary>
        /// Copies an entry to another slot.
        /// </summary>
        /// <param name="from">The slot to copy from</param>
        /// <param name="to">The slot to copy to</param>
        /// <returns>true if something was overwritten.</returns>
        public async Task<bool> Copy(int from, int to)
        {
            if (from <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            if (to <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("copy", from, to))[0];
        }
        /// <summary>
        /// Copies an entry to another slot to another database.
        /// </summary>
        /// <param name="from">The slot to copy from</param>
        /// <param name="to">The slot to copy to</param>
        /// <param name="database">Destination database</param>
        /// <returns>true if something was overwritten.</returns>
        public async Task<bool> Copy(int from, int to, DatabaseComponent database)
        {
            if (from <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            if (to <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("copy", from, to, database))[0];
        }
        /// <summary>
        /// Copies the data stored in this database to another database.
        /// </summary>
        /// <param name="database">Destination database</param>
        /// <returns>How many entries were copied</returns>
        public async Task<int> Clone(DatabaseComponent database) => (await Invoke("clone", database))[0];
    }
}
