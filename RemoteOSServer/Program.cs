using EasyJSON;
using RemoteOS;
using RemoteOS.OpenComputers;
using RemoteOS.OpenComputers.Components;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;

BlockingCollection<DroneComponent> drones = new();

var server = new RemoteOSServer(IPAddress.Any, 4466);
server.onConnected += async (Machine machine) => {
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("New connection");
    Console.ResetColor();
    try
    {
        if(machine.Components.TryGet<ScreenComponent>(out var screen) && machine.Components.TryGet<GraphicsCardComponent>(out var gpu))
        {
            if(await gpu.Bind(screen))
            {
                await gpu.Set(1, 1, "Hello from RemoteOS!");
                await gpu.Copy(1, 1, 20, 1, 0, 1);
                await gpu.SetForeground(Color.Red);
                await gpu.Set(1, 3, "R");
                await gpu.SetForeground(Color.Orange);
                await gpu.Set(2, 3, "A");
                await gpu.SetForeground(Color.Yellow);
                await gpu.Set(3, 3, "I");
                await gpu.SetForeground(Color.Lime);
                await gpu.Set(4, 3, "N");
                await gpu.SetForeground(Color.Cyan);
                await gpu.Set(5, 3, "B");
                await gpu.SetForeground(Color.Blue);
                await gpu.Set(6, 3, "O");
                await gpu.SetForeground(Color.Magenta);
                await gpu.Set(7, 3, "W");
                await gpu.SetForeground(Color.Black);
                await gpu.SetBackground(Color.White);
                await gpu.Set(1, 4, "This is a GPU component test...");
                await gpu.SetForeground(Color.White);
                await gpu.SetBackground(Color.Black);
                if (machine.Components.TryGet<KeyboardComponent>(out var keyboard))
                {
                    keyboard.KeyDown += async (c, key, player) =>
                    {
                        await gpu.Set(1, 5, $"{player} pressed key '{c}' ({key})");
                    };
                }
            }
        }
        if (machine.Components.TryGet<DroneComponent>(out var drone))
        {
            drones.Add(drone);
        }
    }
    catch (Exception) { }
};
server.onDisconnected += async (machine) => {
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine("Connection lost");
    Console.ResetColor();
};
server.Start();
Console.WriteLine("Server started");

//Sample syncronization programm, place 12 drones along X axis on each block and start them one by one from left to right
Vector3[] IcoSphere = BuildIcoSphere();
ConcurrentDictionary<DroneComponent, Vector3> DronePos = new();
ConcurrentDictionary<DroneComponent, Vector3> NextPos = new();
while (drones.Count < IcoSphere.Length) Thread.Sleep(0);
var i = 0;
foreach (var drone in drones)
{
    NextPos[drone] = IcoSphere[i] * 2;
    NextPos[drone] = new((float)RoundToQuarter(NextPos[drone].X), (float)RoundToQuarter(NextPos[drone].Y), (float)RoundToQuarter(NextPos[drone].Z));
    DronePos[drone] = new(i++ - IcoSphere.Length / 2, -10, 0);
}
while (true)
{
    drones.AsParallel().ForAll(async (drone) => {
        var mtx = RoundToQuarter(NextPos[drone].X) - RoundToQuarter(DronePos[drone].X);
        var mty = RoundToQuarter(NextPos[drone].Y) - RoundToQuarter(DronePos[drone].Y);
        var mtz = RoundToQuarter(NextPos[drone].Z) - RoundToQuarter(DronePos[drone].Z);
        await drone.MoveSync(mtx, mty, mtz);
        DronePos[drone] = NextPos[drone];
        if (Math.Round(await drone.GetAcceleration(), 2) != .5) await drone.SetAcceleration(.5);
    });
    while (DronePos.Any(x => x.Value != NextPos[x.Key])) Thread.Sleep(0);
    foreach (var drone in drones)
        NextPos[drone] = Vector3.Transform(NextPos[drone], Quaternion.CreateFromYawPitchRoll(0.25f, 0.25f, 0));
}

Vector3[] BuildIcoSphere()
{
    float t = (float)((1.0f + Math.Sqrt(5.0f)) / 2.0f);

    return new Vector3[] {
        new Vector3(-1, t, 0),
        new Vector3(1, t, 0),
        new Vector3(-1, -t, 0),
        new Vector3(1, -t, 0),

        new Vector3(0, -1, t),
        new Vector3(0, 1, t),
        new Vector3(0, -1, -t),
        new Vector3(0, 1, -t),

        new Vector3(t, 0, -1),
        new Vector3(t, 0, 1),
        new Vector3(-t, 0, -1),
        new Vector3(-t, 0, 1)
    };
}

double RoundToQuarter(double a) => Math.Round(a * 4, MidpointRounding.ToEven) / 4;