using RemoteOS.Helpers;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("cipher")]
    public partial class CipherBlockComponent : Component
    {
        public CipherBlockComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Encrypts the specified message
        /// </summary>
        /// <param name="message">Original message</param>
        /// <returns>Encrypted message</returns>
        public partial Task<string> Encrypt(string message);

        /// <summary>
        /// Decrypts the specified message
        /// </summary>
        /// <param name="message">Encrypted message</param>
        /// <returns>Original message</returns>
        public partial Task<string> Decrypt(string message);

        /// <returns>Whether the block is currently locked</returns>
        public partial Task<bool> IsLocked();

        /// <summary>
        /// Sets whether the block is currently locked
        /// </summary>
        /// <param name="locked">Is locked</param>
        public partial Task SetLocked(bool locked);

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public bool Locked {
            get => IsLocked().Result;
            set => SetLocked(value);
        }
#endif
    }
}