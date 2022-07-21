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

        public async Task<(bool Success, string Reason, int Amount)> GetTankCapacity(Sides side)
        {
            var res = await Invoke("getTankCapacity", side);
            return (res[0].IsNumber, res[1], res[0]);
        }

        public async Task<(bool Success, string Reason, int Amount)> GetTankCapacity(Sides side, int slot)
        {
            if (slot <= 0) throw new InventoryException("There is no such tank");
            var res = await Invoke("getTankCapacity", side, slot);
            return (res[0].IsNumber, res[1], res[0]);
        }

        public async Task<(bool Success, string Reason, int Amount)> GetTankLevel(Sides side)
        {
            var res = await Invoke("getTankLevel", side);
            return (res[0].IsNumber, res[1], res[0]);
        }

        public async Task<(bool Success, string Reason, int Amount)> GetTankLevel(Sides side, int slot)
        {
            if (slot <= 0) throw new InventoryException("There is no such tank");
            var res = await Invoke("getTankLevel", side, slot);
            return (res[0].IsNumber, res[1], res[0]);
        }

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

        public async Task<FluidInfo> GetFluidInInternalTank(int slot)
        {
            if (slot <= 0 || (Parent.Components.TryGet<Agent>(out var a) && slot > await a.GetTankCount())) throw new InventoryException("There is no such tank");
            var res = await Invoke("getFluidInInternalTank", slot);
            return new()
            {
                Amount = res["amount"],
                HasTag = res["hasTag"],
                Label = res["label"],
                Name = res["name"]
            };
        }

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

        public async Task<bool> Drain(int amount) => (await Invoke("drain", amount))[0];

        public async Task<bool> Drain() => (await Invoke("drain"))[0];

        public async Task<bool> Fill(int amount) => (await Invoke("fill", amount))[0];

        public async Task<bool> Fill() => (await Invoke("fill"))[0];

        public async Task<int> GetTankCapacityInSlot(int slot)
        {
            if (slot <= 0 || (Parent.Components.TryGet<Agent>(out var a) && slot > await a.GetTankCount())) throw new InventoryException("There is no such tank");
            var res = await Invoke("getTankCapacityInSlot", slot);
            return res[0];
        }

        public async Task<int> GetTankCapacityInSlot() => (await Invoke("getTankCapacityInSlot"))[0];

        public async Task<int> GetTankLevelInSlot(int slot)
        {
            if (slot <= 0 || (Parent.Components.TryGet<Agent>(out var a) && slot > await a.GetTankCount())) throw new InventoryException("There is no such tank");
            var res = await Invoke("getTankLevelInSlot", slot);
            return res[0];
        }

        public async Task<int> GetTankLevelInSlot() => (await Invoke("getTankLevelInSlot"))[0];

        public async Task<FluidInfo> GetFluidInTankInSlot(int slot)
        {
            if (slot <= 0 || (Parent.Components.TryGet<Agent>(out var a) && slot > await a.GetTankCount())) throw new InventoryException("There is no such tank");
            var res = await Invoke("getFluidInTankInSlot", slot);
            return new()
            {
                Amount = res["amount"],
                HasTag = res["hasTag"],
                Label = res["label"],
                Name = res["name"]
            };
        }

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
