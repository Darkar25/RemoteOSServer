using System.Numerics;
using RemoteOSServer.OpenComputers.Data;

namespace RemoteOS.OpenComputers.Components.OpenSecurity
{
    [Component("os_entdetector")]
    public class EntityDetectorComponent : Component
    {
        public EntityDetectorComponent(Machine parent, Guid address) : base(parent, address)
        {
            parent.Listen("entityDetect", (parameters) =>
            {
                if (parameters[0].Value == Address.ToString())
                    EntityDetect?.Invoke((Guid) Guid.Parse(parameters[0]), (string) parameters[1],
                        (Vector3) new Vector3(parameters[2], parameters[3], parameters[4]));
            });
        }

        /// <summary>
        /// A specific event that is called after calling the scan Entities or scanPlayers method
        /// <para> Parameters:
        /// <br>GUID - address of component</br>
        /// <br>string - name of entity</br>
        /// <br>Vector3 - coordinates of entity</br>
        /// </para>
        /// </summary>
        public event Action<Guid, string, Vector3>? EntityDetect;

        /// <summary>
        /// Get current coordinates of entity detector
        /// </summary>
        /// <returns>Coordinates in Vector3 format</returns>
        public async Task<Vector3> GetLocation()
        {
            var location = await Invoke("getLoc");
            return new Vector3(location[0], location[1], location[2]);
        }

        /// <summary>
        /// Get all entities in within range of entity detector
        /// </summary>
        /// <param name="range">radius of action</param>
        /// <returns>Entity List</returns>
        public async Task<IEnumerable<OsEntity>> ScanEntities(int range = 64)
        {
            return (await Invoke("scanEntities", range))[0].Linq
                .Select(entity => new OsEntity(entity.Value["name"], entity.Value["range"], entity.Value["x"],
                    entity.Value["y"], entity.Value["z"])).ToList();
        }

        /// <summary>
        /// Get all players in within range of entity detector
        /// </summary>
        /// <param name="range">radius of action</param>
        /// <returns>Players List</returns>
        public async Task<IEnumerable<OsEntity>> ScanPlayers(int range = 64)
        {
            return (await Invoke("scanPlayers", range))[0].Linq
                .Select(player => new OsEntity(player.Value["name"], player.Value["range"], player.Value["x"],
                    player.Value["y"], player.Value["z"])).ToList();
        }
    }
}