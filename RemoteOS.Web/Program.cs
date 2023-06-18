using EasyJSON;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteOS;
using RemoteOS.OpenComputers.Components;
using RemoteOS.OpenComputers.Data;
using RemoteOS.Helpers;
using RemoteOS.Web;
using RemoteOS.Web.Database;
using RemoteOS.Web.Hubs;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddDbContext<ROSContext>(options =>
	options
	.UseSqlite(builder.Configuration.GetConnectionString("ROSContext"))
	.EnableSensitiveDataLogging()
);
builder.Services.AddSingleton<MachineManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
});

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<MainHub>("/hub/main");

var machineManager = app.Services.GetRequiredService<MachineManager>();
machineManager.ComputerReady += async (entry) => {
	try
	{
		var machine = machineManager.Get(entry.Address)!;
		// `cuz the pushSignal does not support value tables in lua 5.3
		if ((await machine.Computer.GetArchitecture()) != "Lua 5.2")
		{
			await machine.Computer.SetArchitecture("Lua 5.2");
			return;
		}
		var components = await machine.GetComponents();
		if (components.TryGet<Agent>(out var agent))
		{
			if (agent is RobotComponent robot)
			{
				if (components.TryGet<GeolyzerComponent>(out var geolyzer1))
				{
					await robot.Hook("turn", (original) => $"local r={original}(...)if r then computer.pushSignal(\"ROS:robot_turn\",...,{geolyzer1.GetHandle().Result}.analyze({Sides.Front.Luaify()}))end return r");
					await robot.Hook("move", (original) => $"local r={original}(...)if r then local p={{...}}local s={{0,1,2,3,4,5}}table.remove(s,p[1]%2==0 and p[1]+2 or p[1])local a={{}}for k,v in pairs(s) do table.insert(a,{geolyzer1.GetHandle().Result}.analyze(v))end computer.pushSignal(\"ROS:robot_move\",...,{{table.unpack({geolyzer1.GetHandle().Result}.scan({ROSUtils.scanOrigin.X},{ROSUtils.scanOrigin.Z},{ROSUtils.scanOrigin.Y},{ROSUtils.scanSize.X},{ROSUtils.scanSize.Z},{ROSUtils.scanSize.Y}))}},json.encode(a),s)end return r");
				}
				else
				{
					await robot.SignalHook("turn", "ROS:robot_turn");
					await robot.SignalHook("move", "ROS:robot_move");
				}
				machine.Listen("ROS:robot_turn", async (parameters) =>
				{
					using var scope = machineManager.scopeFactory.CreateScope();
					using var context = scope.ServiceProvider.GetRequiredService<ROSContext>();
					var facing = entry.Facing;
					if (parameters[0].AsBool)
						facing = facing switch
						{
							Sides.Front => Sides.Right,
							Sides.Right => Sides.Back,
							Sides.Back => Sides.Left,
							Sides.Left => Sides.Front,
							_ => throw new Exception("This should never happen")
						};
					else
						facing = facing switch
						{
							Sides.Front => Sides.Left,
							Sides.Right => Sides.Front,
							Sides.Back => Sides.Right,
							Sides.Left => Sides.Back,
							_ => throw new Exception("This should never happen")
						};
					context.Attach(entry);
					entry.Facing = facing;
					if (!parameters[1].IsNull)
					{
						var block = new AnalyzedBlock(GeolyzerResult.FromJson(parameters[1]))
						{
							WorldPosition = ROSUtils.SidePosition(entry)
						};
						await context.World.Where(x => x.X == block.X && x.Y == block.Y && x.Z == block.Z).ExecuteDeleteAsync();
						if (!(block.ModName == "minecraft" && block.Name == "air"))
							context.World.Add(block);
					}
					await context.SaveChangesAsync();
					await machineManager.SendPositionToAll(entry);
				});
				machine.Listen("ROS:robot_move", async (parameters) =>
				{
					using var scope = machineManager.scopeFactory.CreateScope();
					using var context = scope.ServiceProvider.GetRequiredService<ROSContext>();
					context.Attach(entry);
					entry.WorldPosition = entry.SidePosition((Sides)parameters[0].AsInt);
					await context.SaveChangesAsync();
					await machineManager.SendPositionToAll(entry);
					if (!parameters[1].IsNull)
					{
						var sides = new List<Sides>(new[] { Sides.Bottom, Sides.Top, Sides.Back, Sides.Front, Sides.Right, Sides.Left });
						sides.Remove(((Sides)parameters[0].AsInt).Opposite());
						var analyzed = JSONNode.Parse(parameters[2].Value).AsArray.Linq.Select(x => GeolyzerResult.FromJson(x.Value));
						var scanresult = GeolyzerComponent.TransformScannedArray(parameters[1].Linq.Select(x => x.Value.AsDouble).ToArray(), (int)ROSUtils.scanSize.X, (int)ROSUtils.scanSize.Y, (int)ROSUtils.scanSize.Z);
						await machineManager.AnalyzeWorld(entry, sides, analyzed, scanresult);
					}
				});
			}
			else if (agent is DroneComponent drone)
			{
				machine.Listen("ROS:drone_move", async (parameters) =>
				{
					using var scope = machineManager.scopeFactory.CreateScope();
					using var context = scope.ServiceProvider.GetRequiredService<ROSContext>();
					context.Attach(entry);
					entry.X += parameters[0].AsFloat;
					entry.Y += parameters[1].AsFloat;
					entry.Z += parameters[2].AsFloat;
					await context.SaveChangesAsync();
					await machineManager.SendPositionToAll(entry);
					if (!parameters[3].IsNull)
					{
						var analyzed = parameters[4].AsArray.Linq.Select(x => GeolyzerResult.FromJson(x.Value));
						var scanresult = GeolyzerComponent.TransformScannedArray(parameters[3].Linq.Select(x => x.Value.AsDouble).ToArray(), (int)ROSUtils.scanSize.X, (int)ROSUtils.scanSize.Y, (int)ROSUtils.scanSize.Z);
						await machineManager.AnalyzeWorld(entry, new Sides[] { Sides.Up, Sides.Down, Sides.Back, Sides.Right, Sides.Front, Sides.Left }, analyzed, scanresult);
					}
				});
			}
			if (components.TryGet<GeolyzerComponent>(out var geolyzer))
			{
				var sides = new Sides[] { Sides.Up, Sides.Down, Sides.Back, Sides.Right, Sides.Front, Sides.Left };
				await machineManager.AnalyzeWorld(entry, sides, await geolyzer.BulkAnalyze(sides), await geolyzer.Scan((int)ROSUtils.scanOrigin.X, (int)ROSUtils.scanOrigin.Z, (int)ROSUtils.scanOrigin.Y, (int)ROSUtils.scanSize.X, (int)ROSUtils.scanSize.Z, (int)ROSUtils.scanSize.Y));
			}
		}
	}
	catch (OperationCanceledException) { }
};

var server = new RemoteOSServer(IPAddress.Any, 4466);
server.onConnected += machineManager.Connect;
server.onDisconnected += machineManager.Disconnect;
server.Start();

app.Run();
