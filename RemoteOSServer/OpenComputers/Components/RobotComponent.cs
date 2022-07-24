using RemoteOS.OpenComputers.Exceptions;
using System.Drawing;

namespace RemoteOS.OpenComputers.Components
{
    [Component("robot")]
    public class RobotComponent : Agent
    {
        public RobotComponent(Machine parent, Guid address) : base(parent, address) { }

        /// <summary>
        /// Move in the specified direction.
        /// </summary>
        /// <param name="direction">Direction for movement</param>
        /// <returns>Whether the movement was successful, and the reason why if it wasnt</returns>
        /// <exception cref="InvalidSideException">This side if not supported</exception>
        public async Task<(bool Success, string Reason)> Move(Sides direction)
        {
            if ((int)direction > 3) throw new InvalidSideException("Direction can be only Bottom, Top, Back, Front");
            var cmd = await Invoke("move", direction);
            return (cmd[0], cmd[1]);
        }
        /// <summary>
        /// Rotate in the specified direction.
        /// </summary>
        /// <param name="clockwise">Turn clockwise</param>
        /// <returns>true if rotated successfully</returns>
        public async Task<bool> Turn(bool clockwise) => (await Invoke("turn", clockwise))[0];
        /// <returns>The durability of the currently equipped tool.</returns>
        public async Task<float> GetDurability() => (await Invoke("durability"))[0] ?? 0f;
#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public float Durabitliy => GetDurability().Result;
#endif
    }
}
