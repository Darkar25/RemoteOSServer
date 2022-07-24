using RemoteOS.OpenComputers.Data;
using RemoteOS.OpenComputers.Exceptions;

namespace RemoteOS.OpenComputers.Components
{
    [Component("tank_controller")]
    public class TankControllerComponent : Component
    {
        public TankControllerComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <inheritdoc cref="GetTankCapacity(Sides, int)"/>
        public async Task<(bool Success, string Reason, int Amount)> GetTankCapacity(Sides side)
        {
            var res = await Invoke("getTankCapacity", side);
            return (res[0].IsNumber, res[1], res[0]);
        }
        /// <param name="side">The side to analyze</param>
        /// <param name="slot">The slot to analyze</param>
        /// <returns>The capacity of the tank on the specified side.</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<(bool Success, string Reason, int Amount)> GetTankCapacity(Sides side, int slot)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            var res = await Invoke("getTankCapacity", side, slot);
            return (res[0].IsNumber, res[1], res[0]);
        }
        /// <inheritdoc cref="GetTankLevel(Sides, int)"/>
        public async Task<(bool Success, string Reason, int Amount)> GetTankLevel(Sides side)
        {
            var res = await Invoke("getTankLevel", side);
            return (res[0].IsNumber, res[1], res[0]);
        }
        /// <param name="side">The side to analyze</param>
        /// <param name="slot">The slot to analyze</param>
        /// <returns>The amount of fluid in the tank on the specified side.</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<(bool Success, string Reason, int Amount)> GetTankLevel(Sides side, int slot)
        {
            if (slot <= 0) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            var res = await Invoke("getTankLevel", side, slot);
            return (res[0].IsNumber, res[1], res[0]);
        }
        /// <inheritdoc cref="GetFluidInTank(Sides, int)"/>
        public async Task<FluidInfo> GetFluidInTank(Sides side)
        {
            var res = await Invoke("getFluidInTank", side);
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
        public async Task<FluidInfo> GetFluidInTank(Sides side, int tank)
        {
            var res = await Invoke("getFluidInTank", side);
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
            if (slot <= 0 || (Parent.Components.TryGet<Agent>(out var a) && slot > await a.GetTankCount())) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            var res = await Invoke("getFluidInInternalTank", slot);
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
            var res = await Invoke("getFluidInInternalTank");
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
        public async Task<bool> Drain(int amount) => (await Invoke("drain", amount))[0];
        /// <inheritdoc cref="Drain(int)"/>
        public async Task<bool> Drain() => (await Invoke("drain"))[0];
        /// <summary>
        /// Transfers fluid from the selected tank to a tank in the selected inventory slot.
        /// </summary>
        /// <param name="amount">How much to transfer</param>
        /// <returns>true if fluid was transfered</returns>
        public async Task<bool> Fill(int amount) => (await Invoke("fill", amount))[0];
        /// <inheritdoc cref="Fill(int)"/>
        public async Task<bool> Fill() => (await Invoke("fill"))[0];
        /// <param name="slot">The slot to analyze</param>
        /// <returns>the capacity of the tank item in the specified slot of the robot or the selected slot.</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<int> GetTankCapacityInSlot(int slot)
        {
            if (slot <= 0 || (Parent.Components.TryGet<Agent>(out var a) && slot > await a.GetTankCount())) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            var res = await Invoke("getTankCapacityInSlot", slot);
            return res[0];
        }
        /// <inheritdoc cref="GetTankCapacityInSlot(int)"/>
        public async Task<int> GetTankCapacityInSlot() => (await Invoke("getTankCapacityInSlot"))[0];
        /// <param name="slot">The slot to analyze</param>
        /// <returns>The amount of fluid in the tank item in the specified slot or the selected slot.</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<int> GetTankLevelInSlot(int slot)
        {
            if (slot <= 0 || (Parent.Components.TryGet<Agent>(out var a) && slot > await a.GetTankCount())) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            var res = await Invoke("getTankLevelInSlot", slot);
            return res[0];
        }
        /// <inheritdoc cref="GetTankLevelInSlot(int)"/>
        public async Task<int> GetTankLevelInSlot() => (await Invoke("getTankLevelInSlot"))[0];
        /// <param name="slot">The slot to analyze</param>
        /// <returns>A description of the fluid in the tank item in the specified slot or the selected slot.</returns>
        /// <exception cref="InventoryException">This tank does not exist</exception>
        public async Task<FluidInfo> GetFluidInTankInSlot(int slot)
        {
            if (slot <= 0 || (Parent.Components.TryGet<Agent>(out var a) && slot > await a.GetTankCount())) throw new InventoryException(InventoryException.NO_SUCH_TANK);
            var res = await Invoke("getFluidInTankInSlot", slot);
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
            var res = await Invoke("getFluidInTankInSlot");
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
