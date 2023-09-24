using OneOf;
using RemoteOS.OpenComputers.Data;
using RemoteOS.OpenComputers.Exceptions;
using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("tank_controller")]
    [Tier(Tier.Two)]
    public partial class TankControllerComponent : Component
    {
        public TankControllerComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <inheritdoc cref="GetTankCapacity(Sides, int)"/>
        public async Task<ReasonOr<int>> GetTankCapacity(Sides side)
        {
            var res = await GetInvoker()(side);
            if (!res[0].IsNumber) return res[1].Value;
            return res[0].AsInt;
        }

        /// <param name="side">The side to analyze</param>
        /// <param name="slot">The slot to analyze</param>
        /// <returns>The capacity of the tank on the specified side.</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<ReasonOr<int>> GetTankCapacity(Sides side, int slot)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            var res = await GetInvoker()(side, slot);
			if (!res[0].IsNumber) return res[1].Value;
			return res[0].AsInt;
		}

        /// <inheritdoc cref="GetTankLevel(Sides, int)"/>
        public async Task<ReasonOr<int>> GetTankLevel(Sides side)
        {
            var res = await GetInvoker()(side);
			if (!res[0].IsNumber) return res[1].Value;
			return res[0].AsInt;
		}

        /// <param name="side">The side to analyze</param>
        /// <param name="slot">The slot to analyze</param>
        /// <returns>The amount of fluid in the tank on the specified side.</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<OneOf<int, string>> GetTankLevel(Sides side, int slot)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            var res = await GetInvoker()(side, slot);
			if (!res[0].IsNumber) return res[1].Value;
			return res[0].AsInt;
		}

        /// <inheritdoc cref="GetFluidInTank(Sides, int)"/>
        public async Task<FluidInfo> GetFluidInTank(Sides side)
        {
            var res = await GetInvoker()(side);
            return new()
            {
                Amount = res["amount"],
                HasTag = res["hasTag"],
                Label = res["label"],
                Name = res["name"]
            };
        }

        /// <param name="side">The side to analyze</param>
        /// <param name="slot">The slot to analyze</param>
        /// <returns>A description of the fluid in the the tank on the specified side.</returns>
        public async Task<FluidInfo> GetFluidInTank(Sides side, int slot)
        {
            var res = await GetInvoker()(side, slot);
            return new()
            {
                Amount = res["amount"],
                HasTag = res["hasTag"],
                Label = res["label"],
                Name = res["name"]
            };
        }

        /// <param name="slot">The slot to analyze</param>
        /// <returns>A description of the fluid in the tank in the specified slot or the selected slot.</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<FluidInfo> GetFluidInInternalTank(int slot)
        {
            if (slot <= 0 || ((await Parent.GetComponents()).TryGet<Agent>(out var a) && slot > await a.GetTankCount())) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            var res = await GetInvoker()(slot);
            return new()
            {
                Amount = res["amount"],
                HasTag = res["hasTag"],
                Label = res["label"],
                Name = res["name"]
            };
        }

        /// <inheritdoc cref="GetFluidInInternalTank(int)"/>
        public async Task<FluidInfo> GetFluidInInternalTank()
        {
            var res = await GetInvoker()();
            return new()
            {
                Amount = res["amount"],
                HasTag = res["hasTag"],
                Label = res["label"],
                Name = res["name"]
            };
        }

        /// <summary>
        /// Transfers fluid from a tank in the selected inventory slot to the selected tank.
        /// </summary>
        /// <param name="amount">How much to transfer</param>
        /// <returns>true if fluid was transfered</returns>
        public partial Task<bool> Drain(int amount);

        /// <inheritdoc cref="Drain(int)"/>
        public partial Task<bool> Drain();

        /// <summary>
        /// Transfers fluid from the selected tank to a tank in the selected inventory slot.
        /// </summary>
        /// <param name="amount">How much to transfer</param>
        /// <returns>true if fluid was transfered</returns>
        public partial Task<bool> Fill(int amount);

        /// <inheritdoc cref="Fill(int)"/>
        public partial Task<bool> Fill();

        /// <param name="slot">The slot to analyze</param>
        /// <returns>the capacity of the tank item in the specified slot of the robot or the selected slot.</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<int> GetTankCapacityInSlot(int slot)
        {
            if (slot <= 0 || ((await Parent.GetComponents()).TryGet<Agent>(out var a) && slot > await a.GetTankCount())) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            return (await GetInvoker()(slot))[0];
        }

        /// <inheritdoc cref="GetTankCapacityInSlot(int)"/>
        public partial Task<int> GetTankCapacityInSlot();

        /// <param name="slot">The slot to analyze</param>
        /// <returns>The amount of fluid in the tank item in the specified slot or the selected slot.</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<int> GetTankLevelInSlot(int slot)
        {
            if (slot <= 0 || ((await Parent.GetComponents()).TryGet<Agent>(out var a) && slot > await a.GetTankCount())) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            return (await GetInvoker()(slot))[0];
        }

        /// <inheritdoc cref="GetTankLevelInSlot(int)"/>
        public partial Task<int> GetTankLevelInSlot();

        /// <param name="slot">The slot to analyze</param>
        /// <returns>A description of the fluid in the tank item in the specified slot or the selected slot.</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<FluidInfo> GetFluidInTankInSlot(int slot)
        {
            if (slot <= 0 || ((await Parent.GetComponents()).TryGet<Agent>(out var a) && slot > await a.GetTankCount())) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            var res = await GetInvoker()(slot);
            return new()
            {
                Amount = res["amount"],
                HasTag = res["hasTag"],
                Label = res["label"],
                Name = res["name"]
            };
        }

        /// <inheritdoc cref=" GetFluidInTankInSlot(int)"/>
        public async Task<FluidInfo> GetFluidInTankInSlot()
        {
            var res = await GetInvoker()();
            return new()
            {
                Amount = res["amount"],
                HasTag = res["hasTag"],
                Label = res["label"],
                Name = res["name"]
            };
        }
    }
}
