using OneOf.Types;
using RemoteOS.Helpers;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("speech_box"), Component("speech")]
    public partial class SpeechBoxComponent : Component
    {
        public SpeechBoxComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Say the specified message.
        /// </summary>
        /// <param name="message">The message to say</param>
        public async Task<ReasonOr<Success>> Say(string message)
        {
            var res = await GetInvoker()(message);
            if (res[0]) return new Success();
            return res[1].Value;
        }

        /// <summary>
        /// Stops the currently spoken phrase.
        /// </summary>
        public async Task<ReasonOr<Success>> Stop()
        {
            var res = await GetInvoker()();
            if (res[0]) return new Success();
            return res[1].Value;
        }

        /// <returns>True if the device is currently processing text.</returns>
        public partial Task<bool> IsProcessing();

        /// <summary>
        /// Sets the volume of the speech box. Needs to be beween 0 and 1
        /// </summary>
        /// <param name="volume">The new volume</param>
        /// <exception cref="ArgumentOutOfRangeException">Volume must me a number between 0 and 1</exception>
        public Task SetVolume(double volume)
        {
            if (volume < 0 || volume > 1) throw new ArgumentOutOfRangeException(nameof(volume), "Volume must be a number between 0 and 1");
            return GetInvoker()(volume);
        }

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public bool Processing => IsProcessing().Result;
#endif
    }
}