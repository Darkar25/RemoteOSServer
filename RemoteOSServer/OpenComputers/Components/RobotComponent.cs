using OneOf;
using OneOf.Types;
using RemoteOS.OpenComputers.Exceptions;
using System.Drawing;
using RemoteOS.Helpers;
using RemoteOS.OpenComputers.Data;

namespace RemoteOS.OpenComputers.Components
{
    [Component("robot")]
    public partial class RobotComponent : Agent
    {
        public RobotComponent(Machine parent, Guid address) : base(parent, address) { }

        /// <summary>
        /// Move in the specified direction.
        /// </summary>
        /// <param name="direction">Direction for movement</param>
        /// <returns>Whether the movement was successful, and the reason why if it wasnt</returns>
        /// <exception cref="InvalidSideException">This side if not supported</exception>
        public async Task<ReasonOr<Success>> Move(Sides direction)
        {
            direction.ThrowIfOtherThan(Sides.Front, Sides.Top, Sides.Bottom, Sides.Back);
            var cmd = await Invoke("move", direction);
            if (!cmd[0]) return new Reason(cmd[1].Value);
            return new Success();
        }

        /// <summary>
        /// Rotate in the specified direction.
        /// </summary>
        /// <param name="clockwise">Turn clockwise</param>
        /// <returns>true if rotated successfully</returns>
        public partial Task<bool> Turn(bool clockwise);

        /// <returns>The durability of the currently equipped tool.</returns>
        public async Task<float> GetDurability() => await InvokeFirst("durability");
#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public float Durabitliy => GetDurability().Result;
#endif
    }
}
