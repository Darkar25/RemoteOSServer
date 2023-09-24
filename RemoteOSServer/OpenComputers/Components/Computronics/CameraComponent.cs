using RemoteOS.Helpers;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("camera")]
    public partial class CameraComponent : Component
    {
        public CameraComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <param name="x">X offset</param>
        /// <param name="y">Y offset</param>
        /// <returns>The distance to the block the ray is shot at with the specified x-y offset</returns>
        public partial Task<double> Distance(double x, double y);

        /// <returns>The distance to the block directly in front</returns>
        public partial Task<double> Distance();

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public double this[double x, double y] => Distance(x, y).Result;
#endif
    }
}