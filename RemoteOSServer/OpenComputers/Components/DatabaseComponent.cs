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

        public ItemStackInfo this[int slot]
        {
            get
            {
                if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
                return new ItemStackInfo(Invoke("get", slot).Result[0]);
            }
        }

        public async Task<string> ComputeHash(int slot)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("computeHash", slot))[0];
        }

        public async Task<int> IndexOf(string hash) => (await Invoke("indexOf", $@"""{hash}"""))[0];

        public async Task<bool> CLear(int slot)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_SLOT);
            return (await Invoke("clear", slot))[0];
        }

        public async Task<bool> Copy(int from, int to) => (await Invoke("copy", from, to))[0];
        public async Task<bool> Copy(int from, int to, DatabaseComponent database) => (await Invoke("copy", from, to, $@"""{database.Address}"""))[0];
        public async Task<int> Clone(DatabaseComponent database) => (await Invoke("clone", $@"""{database.Address}"""))[0];
    }
}
