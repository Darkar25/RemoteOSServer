using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteOS.OpenComputers.Components;
using RemoteOS.Web.Database;
using System.Numerics;
using RemoteOS.Helpers;
using RemoteOS.OpenComputers.Data;

namespace RemoteOS.Web.Hubs
{
    public class MainHub : Hub
    {
        readonly IServiceScopeFactory scopeFactory;
        readonly MachineManager manager;
        public MainHub(IServiceScopeFactory factory, MachineManager manager)
        {
            scopeFactory = factory;
            this.manager = manager;
        }
		public async Task GetWorld()
		{
			using var scope = scopeFactory.CreateScope();
			using var context = scope.ServiceProvider.GetRequiredService<ROSContext>();
			await Clients.Caller.SendAsync("World", JsonConvert.SerializeObject(context.World.AsNoTracking().ToArray()));
		}

		public async Task GetAgents()
		{
			foreach (var c in manager.ReadyComputers)
			{
				var machine = manager.Get(c.Address)!;
				await Clients.Caller.SendAsync("AgentPosition", JsonConvert.SerializeObject(c));
				await manager.SendBasicRender(machine, Clients.Caller);
            }
		}

		public async Task SetPosition(Guid address, Vector3 pos, Sides facing)
		{
			if (manager.TryGet(address, out var machine))
				await manager.SetPosition(machine, pos, facing);
			else {
                using var scope = scopeFactory.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<ROSContext>();
                var entry = await context.Computers.FirstAsync(x => x.Address == address);
				if (entry is not null)
				{
					entry.WorldPosition = pos;
					entry.Facing = facing;
					await context.SaveChangesAsync();
				}
            }
		}

		public async Task MoveAction(Guid address, string action)
		{
			if (manager.TryGet(address, out var machine))
			{
				var components = await machine.GetComponents();
				if (components.TryGet<Agent>(out var agent))
					if (agent is RobotComponent robot)
						switch (action)
						{
							case "forward": await robot.Forward(); break;
							case "back": await robot.Back(); break;
							case "up": await robot.Up(); break;
							case "down": await robot.Down(); break;
							case "left": await robot.TurnLeft(); break;
							case "right": await robot.TurnRight(); break;
						}
					else if (agent is DroneComponent drone)
					{
						var side = action switch
						{
							"forward" => Sides.Forward,
							"back" => Sides.Back,
							"up" => Sides.Up,
							"down" => Sides.Down,
							"left" => Sides.Left,
							"roght" => Sides.Right,
							_ => throw new ArgumentException("Invalid side", nameof(action))
						};
						await drone.MoveSync(side);
						var direction = ROSUtils.RelativeSidePosition(targetFacing: side);
						if (components.TryGet<GeolyzerComponent>(out var geolyzer))
							await machine.RawExecute($"computer.pushSignal(\"ROS:drone_move\",{direction.X},{direction.Y},{direction.Z},{geolyzer.GetHandle().Result}.scan({ROSUtils.scanOrigin.X},{ROSUtils.scanOrigin.Z},{ROSUtils.scanOrigin.Y},{ROSUtils.scanSize.X},{ROSUtils.scanSize.Z},{ROSUtils.scanSize.Y}),{string.Join(",", new Sides[] { Sides.Up, Sides.Down, Sides.Back, Sides.Right, Sides.Front, Sides.Left }.Select(x => $"{geolyzer.GetHandle().Result}.analyze({x.Luaify()})"))})");
						else
							await machine.RawExecute($"computer.pushSignal(\"ROS:drone_move\",{direction.X},{direction.Y},{direction.Z})");
					}
			}
		}
	}
}
