using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteOS.OpenComputers;
using RemoteOS.OpenComputers.Components;
using RemoteOS.OpenComputers.Data;
using RemoteOS.Web.Database;
using RemoteOS.Web.Hubs;
using RemoteOS.Helpers;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Z.EntityFramework.Plus;

namespace RemoteOS.Web
{
    public class MachineManager
    {
        private Dictionary<Guid, Machine> machines = new();
        public event Action<ComputerDBEntry>? ComputerReady;
        public IServiceScopeFactory scopeFactory;
        public IHubContext<MainHub> hub;

        public MachineManager(IServiceScopeFactory scopeFactory, IHubContext<MainHub> hub)
        {
            this.scopeFactory = scopeFactory;
            this.hub = hub;
        }

        public async void Connect(Machine machine)
        {
            Console.WriteLine("New connection");
            using var scope = scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ROSContext>();
            var address = await machine.Computer.GetAddress();
            machines.Add(address, machine);
            await hub.Clients.All.SendAsync("Connected", address);
            var entry = await context.Computers.AsNoTracking().FirstOrDefaultAsync(x => x.Address == address);
            if (entry is not null)
            {
                if (entry.Facing.HasValue && entry.WorldPosition.HasValue)
                {
                    ComputerReady?.Invoke(entry);
                    await SendPositionToAll(entry);
                    await SendBasicRender(machine, hub.Clients.All);
                }
			}
            else
            {
                await context.Computers.AddAsync(new() { Address = address });
                await context.SaveChangesAsync();
            }
        }

        public async void Disconnect(Machine machine)
        {
            Console.WriteLine("Connection lost");
            var address = await machine.Computer.GetAddress();
            machines.Remove(address);
            await hub.Clients.All.SendAsync("Disconnected", address);
        }

        public async Task SetPosition(Machine machine, Vector3 pos, Sides facing)
        {
            using var scope = scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ROSContext>();
            var address = await machine.Computer.GetAddress();
            var entry = await context.Computers.FirstAsync(x => x.Address == address);
            var needReady = !entry.Facing.HasValue || !entry.WorldPosition.HasValue;
            entry.WorldPosition = pos;
            entry.Facing = facing;
            await context.SaveChangesAsync();
            if (needReady)
            {
                ComputerReady?.Invoke(entry);
                await SendBasicRender(machine, hub.Clients.All);
            }
			await SendPositionToAll(entry);
		}

        public Machine? Get(Guid id)
        {
            if (machines.TryGetValue(id, out var ret)) return ret;
            return null;
        }


        public bool TryGet(Guid id, out Machine machine) => machines.TryGetValue(id, out machine);

        public bool IsOnline(ComputerDBEntry entry) => machines.ContainsKey(entry.Address);

        public bool IsReady(ComputerDBEntry entry) => IsOnline(entry) && entry.WorldPosition.HasValue && entry.Facing.HasValue;

        public async Task SendPositionToAll(ComputerDBEntry entry) => await hub.Clients.All.SendAsync("AgentPosition", JsonConvert.SerializeObject(entry));

        public IEnumerable<ComputerDBEntry> ReadyComputers
        {
            get
            {
				using var scope = scopeFactory.CreateScope();
				using var context = scope.ServiceProvider.GetRequiredService<ROSContext>();
                return context.Computers.AsNoTracking().ToArray().Where(IsReady);
			}
        }

        public async Task SendBasicRender(Machine machine, IClientProxy clients)
        {
            var components = await machine.GetComponents();
            if (components.TryGet<Agent>(out var agent))
                await clients.SendAsync("AgentRender", JsonConvert.SerializeObject(new
                {
                    Address = await machine.Computer.GetAddress(),
                    DisplayName = await agent.GetName(),
                    Color = (await agent.GetLightColor()).ToArgb(),
                    Type = agent.Type
                }));
            else
                await clients.SendAsync("AgentRender", JsonConvert.SerializeObject(new
                {
                    Address = await machine.Computer.GetAddress(),
                    Type = components.IsAvailable<MicrocontrollerComponent>() ? "microcontroller" : "computer"
                }));
        }

        public async Task AnalyzeWorld(ComputerDBEntry entry, IEnumerable<Sides> sides, IEnumerable<GeolyzerResult> analyzed, double[,,] scanresult)
        {
            using var scope = scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ROSContext>();
            var blocks = sides.Zip(analyzed).Select(x => new AnalyzedBlock(x.Second) { WorldPosition = entry.SidePosition(x.First) }).ToArray();
            var occupiedBlocks = context.World.Where(x =>
            x.X >= entry.X!.Value + ROSUtils.scanOrigin.X &&
            x.X < entry.X!.Value + ROSUtils.scanOrigin.X + ROSUtils.scanSize.X &&
            x.Y >= entry.Y!.Value + ROSUtils.scanOrigin.Y &&
            x.Y < entry.Y!.Value + ROSUtils.scanOrigin.Y + ROSUtils.scanSize.Y &&
            x.Z >= entry.Z!.Value + ROSUtils.scanOrigin.Z &&
            x.Z < entry.Z!.Value + ROSUtils.scanOrigin.Z + ROSUtils.scanSize.Z).ToList();
            var occupiedAnalyzedBlocks = occupiedBlocks.Where(b => blocks.Any(x => x.WorldPosition == b.WorldPosition)).ToArray();
            var nonEmptyBlocks = blocks.Where(x => !(x.ModName == "minecraft" && x.Name == "air")).ToArray();
            var analyzedSides = sides.Select(x => ROSUtils.RelativeSidePosition(entry.Facing!.Value, x)).ToList();
            List<IBlock> scanned = new();
            context.World.RemoveRange(occupiedAnalyzedBlocks);
            for (int x = 0; x < scanresult.GetLength(0); x++)
                for (int y = 0; y < scanresult.GetLength(1); y++)
                    for (int z = 0; z < scanresult.GetLength(2); z++)
                    {
                        if (x == -ROSUtils.scanOrigin.X && y == -ROSUtils.scanOrigin.Y && z == -ROSUtils.scanOrigin.Z) continue; // Its the agent, skip
                        if (analyzedSides.Contains(new(x + ROSUtils.scanOrigin.X, y + ROSUtils.scanOrigin.Y, z + ROSUtils.scanOrigin.Z))) continue; // This block has already been scanned, skip
                        var pos = new Vector3(entry.X!.Value + x + ROSUtils.scanOrigin.X, entry.Y!.Value + y + ROSUtils.scanOrigin.Y, entry.Z!.Value + z + ROSUtils.scanOrigin.Z);
                        var blck = occupiedBlocks.FirstOrDefault(b => b.WorldPosition == pos);
                        if (scanresult[x, y, z] == 0 && blck is not null)
                        {
                            scanned.Add(new AnalyzedBlock() { ModName = "minecraft", Name = "air", WorldPosition = pos });
                            context.World.Remove(blck);
                        }
                        else if (scanresult[x, y, z] != 0)
                            if (blck is null)
                            {
                                var nblck = new ScannedBlock() { WorldPosition = pos, Hardness = (float)scanresult[x, y, z] };
                                scanned.Add(nblck);
                                await context.World.AddAsync(nblck);
                            }
                            else if (blck.GetType() == typeof(ScannedBlock))
                            {
                                var sblck = (ScannedBlock)blck;
                                sblck.Hardness = (float)scanresult[x, y, z];
                                scanned.Add(sblck);
                            }
                    }
            await hub.Clients.All.SendAsync("World", JsonConvert.SerializeObject(scanned.Concat(blocks)));

            // First save, push all scanned blocks to DB and delete any potential conflicting scanned blocks
            // EF does not update Discriminator column so we need to delete the entry first to update its discriminator
            await context.SaveChangesAsync();

            await context.World.AddRangeAsync(nonEmptyBlocks);

            // Second save, push remaining blocks to DB
            await context.SaveChangesAsync();
        }
	}
}
