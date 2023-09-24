using RemoteOS.Helpers;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("tape_drive")]
    public partial class TapeDriveComponent : Component
    {
        public TapeDriveComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <returns>True if the tape drive is empty or the inserted tape has reached its end</returns>
        public partial Task<bool> IsEnd();

        /// <returns>True if there is a tape inserted</returns>
        public partial Task<bool> IsReady();

        /// <returns>The size of the tape, in bytes</returns>
        public partial Task<int> GetSize();

        /// <returns>The position of the tape, in bytes</returns>
        public partial Task<int> GetPosition();

        /// <summary>
        /// Sets the label of the tape.
        /// </summary>
        /// <param name="label">The new label</param>
        /// <returns>The new label, or null if there is no tape inserted</returns>
        public partial Task<string?> SetLabel(string label);

        /// <returns>The current label of the tape, or null if there is no tape inserted</returns>
        public partial Task<string?> GetLabel();

        /// <summary>
        /// Seeks the specified amount of bytes on the tape. Negative values for rewinding.
        /// </summary>
        /// <param name="length">The amount of bytes to seek</param>
        /// <returns>The amount of bytes sought, or null if there is no tape inserted</returns>
        public partial Task<int?> Seek(int length);

        /// <summary>
        /// Reads and returns the specified amount of bytes or a single byte from the tape.
        /// </summary>
        /// <param name="length">The amount of bytes to read</param>
        /// <returns>Null if there is no tape inserted</returns>
        public partial Task<string?> Read(int length = 1);

        /// <summary>
        /// Writes the specified data to the tape if there is one inserted
        /// </summary>
        /// <param name="data">The data to write on to the tape</param>
        public partial Task Write(byte data);

        /// <inheritdoc cref="Write(byte)"/>
        public partial Task Write(string data);

        /// <summary>
        /// Make the Tape Drive start playing the tape.
        /// </summary>
        /// <returns>True on success</returns>
        public partial Task<bool> Play();

        /// <summary>
        /// Make the Tape Drive stop playing the tape.
        /// </summary>
        /// <returns>True on success</returns>
        public partial Task<bool> Stop();

        /// <summary>
        /// Sets the speed of the tape drive. Needs to be beween 0.25 and 2.
        /// </summary>
        /// <param name="speed">The new speed</param>
        /// <returns>True on success</returns>
        /// <exception cref="ArgumentOutOfRangeException">The speed is too high or too slow</exception>
        public async Task<bool> SetSpeed(double speed)
        {
            if (speed < 0.25 || speed > 2) throw new ArgumentOutOfRangeException(nameof(speed), "Speed must be a number between 0.25 and 2");
            return (await GetInvoker()(speed))[0];
        }

        /// <summary>
        /// Sets the volume of the tape drive. Needs to be beween 0 and 1
        /// </summary>
        /// <param name="volume">The new volume</param>
        /// <exception cref="ArgumentOutOfRangeException">Volume is too high or too low</exception>
        public Task SetVolume(double volume)
        {
            if (volume < 0 || volume > 1) throw new ArgumentOutOfRangeException(nameof(volume), "Volume must be a number between 0 and 1");
            return GetInvoker()(volume);
        }

        /// <returns>The current state of the tape drive</returns>
        public partial Task<string> GetState();

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public string? Label
        {
            get => GetLabel().Result;
            set => SetLabel(value);
        }

        public int Size => GetSize().Result;

        public int Position => GetPosition().Result;

        public string State => GetState().Result;
#endif
    }
}