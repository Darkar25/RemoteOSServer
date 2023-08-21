using RemoteOS.Helpers;
using RemoteOSServer.OpenComputers.Data;

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("radar")]
    public class RadarComponent : Component
    {
        public RadarComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Returns All Player in within a given radius specified in blocks from the radar.
        /// </summary>
        /// <param name="distance">radius of scan</param>
        /// <returns>Player List</returns>
        public async Task<IEnumerable<Entity>> GetPlayers(int distance)
        {
            return (await Invoke("getPlayers", distance))[0].Linq
                .Select(player => new Entity(player.Value["name"], player.Value["distance"])).ToList();
        }

        /// <summary>
        /// Return All Enitities within a radius. Its like a GetPlayers or GetMobs, but two for the price of one.
        /// </summary>
        /// <param name="distance">radius of scan</param>
        /// <returns>Enitities List</returns>
        public async Task<IEnumerable<Entity>> GetEntities(int distance)
        {
            return (await Invoke("getEntities", distance))[0].Linq
                .Select(entity => new Entity(entity.Value["name"], entity.Value["distance"])).ToList();
        }

        /// <summary>
        /// Return all items located on the ground within a radius
        /// </summary>
        /// <param name="distance">radius of scan</param>
        /// <returns>Items List</returns>
        public async Task<IEnumerable<Item>> GetItems(int distance)
        {
            return (await Invoke("getItems", distance))[0].Linq.ToArray().Select(item =>
                new Item(item.Value["damage"], item.Value["distance"], item.Value["hasTag"], item.Value["label"],
                    item.Value["size"])).ToList();
        }

        /// <summary>
        /// Return all mob essences placed next to the radar
        /// </summary>
        /// <param name="distance">radius of scan</param>
        /// <returns>Mob List</returns>
        public async Task<IEnumerable<Entity>> GetMobs(int distance)
        {
            return (await Invoke("getMobs", distance))[0].Linq.ToArray()
                .Select(mob => new Entity(mob.Value["name"], mob.Value["distance"])).ToList();
        }
    }
}