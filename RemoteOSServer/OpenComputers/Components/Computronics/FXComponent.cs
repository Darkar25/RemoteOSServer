using OneOf.Types;
using RemoteOS.Helpers;
using System.Numerics;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("particle")]
    public class FXComponent : Component
    {
        public FXComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Spawns a particle effect at the specified relative coordinates optionally with the specified velocity
        /// </summary>
        /// <param name="name">Name of the particle</param>
        /// <param name="xCoord">Relative X position of the particle</param>
        /// <param name="yCoord">Relative Y position of the particle</param>
        /// <param name="zCoord">Relative Z position of the particle</param>
        /// <param name="defaultVelocity">Default velocity in the random direction</param>
        /// <returns>Whether the particle was spawned successfully, and the reason if it wasnt</returns>
        /// <exception cref="ArgumentOutOfRangeException">The name of the particle is too long</exception>
        public async Task<ReasonOr<Success>> Spawn(string name, double xCoord, double yCoord, double zCoord, double defaultVelocity)
        {
            if (name.Length > ushort.MaxValue) throw new ArgumentOutOfRangeException(nameof(name), "Name is too long");
            var ret = await GetInvoker()(name, xCoord, yCoord, zCoord, defaultVelocity);
            if (!ret[0]) return ret[1].Value;
            return new Success();
        }

        /// <inheritdoc cref="Spawn(string, double, double, double, double)"/>
        /// <param name="xVel">Default velocity on the X axis</param>
        /// <param name="yVel">Default velocity on the Y axis</param>
        /// <param name="zVel">Default velocity on the Z axis</param>
        public async Task<ReasonOr<Success>> Spawn(string name, double xCoord, double yCoord, double zCoord, double xVel, double yVel, double zVel)
        {
            if (name.Length > ushort.MaxValue) throw new ArgumentOutOfRangeException(nameof(name), "Name is too long");
            var ret = await GetInvoker()(name, xCoord, yCoord, zCoord, xVel, yVel, zVel);
            if (!ret[0]) return ret[1].Value;
            return new Success();
        }

        /// <inheritdoc cref="Spawn(string, double, double, double, double, double, double)"/>
        public async Task<ReasonOr<Success>> Spawn(string name, double xCoord, double yCoord, double zCoord)
        {
            if (name.Length > ushort.MaxValue) throw new ArgumentOutOfRangeException(nameof(name), "Name is too long");
            var ret = await GetInvoker()(name, xCoord, yCoord, zCoord);
            if (!ret[0]) return ret[1].Value;
            return new Success();
        }

        /// <inheritdoc cref="Spawn(string, double, double, double, double, double, double)"/>
        /// <param name="position">Relative position of the particle</param>
        public Task<ReasonOr<Success>> Spawn(string name, Vector3 position) => Spawn(name, position.X, position.Y, position.Z);

        /// <inheritdoc cref="Spawn(string, Vector3)"/>
        public Task<ReasonOr<Success>> Spawn(string name, Vector3 position, double defaultVelocity) => Spawn(name, position.X, position.Y, position.Z, defaultVelocity);

        /// <inheritdoc cref="Spawn(string, Vector3)"/>
        /// <param name="velocity">Default velocity of the particle</param>
        public Task<ReasonOr<Success>> Spawn(string name, Vector3 position, Vector3 velocity) => Spawn(name, position.X, position.Y, position.Z, velocity.X, velocity.Y, velocity.Z);

        /// <inheritdoc cref="Spawn(string, Vector3, Vector3)"/>
        public Task<ReasonOr<Success>> Spawn(string name, double xCoord, double yCoord, double zCoord, Vector3 velocity) => Spawn(name, xCoord, yCoord, zCoord, velocity.X, velocity.Y, velocity.Z);

        /// <inheritdoc cref="Spawn(string, Vector3, Vector3)"/>
        public Task<ReasonOr<Success>> Spawn(string name, Vector3 position, double xVel, double yVel, double zVel) => Spawn(name, position.X, position.Y, position.Z, xVel, yVel, zVel);
    }
}