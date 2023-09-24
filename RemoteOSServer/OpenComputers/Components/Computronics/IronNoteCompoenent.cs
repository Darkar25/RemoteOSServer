using RemoteOS.Helpers;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("iron_noteblock")]
    public partial class IronNoteCompoenent : Component
    {
        public readonly string[] instruments = new[] { "harp", "bd", "snare", "hat", "bassattack", "pling", "bass" };

        public IronNoteCompoenent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Plays the specified note with the specified instrument; volume may be a number between 0 and 1
        /// </summary>
        /// <param name="instrument">Which instrument to use</param>
        /// <param name="note">The note to paly</param>
        /// <param name="volume">The volume of the played note</param>
        /// <exception cref="ArgumentOutOfRangeException">No such instrument/No such note/Volume is too high or too low</exception>
        public Task PlayNote(int instrument, int note, double volume = 1d)
        {
            if (volume < 0 || volume > 1) throw new ArgumentOutOfRangeException(nameof(volume), "Volume must be a number between 0 and 1");
            if (instrument < 0 || instrument >= instruments.Length) throw new ArgumentOutOfRangeException(nameof(instrument), "Invalid instrument");
            if (note < 0) throw new ArgumentOutOfRangeException(nameof(note), "Note cannot be negative");
            return GetInvoker()(instrument, note, volume);
        }

        /// <inheritdoc cref="PlayNote(int, int, double)"/>
        public Task PlayNote(string instrument, int note, double volume = 1d)
        {
            if (volume < 0 || volume > 1) throw new ArgumentOutOfRangeException(nameof(volume), "Volume must be a number between 0 and 1");
            if (Array.IndexOf(instruments, instrument) == -1) throw new ArgumentOutOfRangeException(nameof(instrument), "Invalid instrument");
            if (note < 0) throw new ArgumentOutOfRangeException(nameof(note), "Note cannot be negative");
            return GetInvoker()(instrument, note, volume);
        }

        /// <inheritdoc cref="PlayNote(int, int, double)"/>
        public Task PlayNote(int note, double volume = 1d)
        {
            if (volume < 0 || volume > 1) throw new ArgumentOutOfRangeException(nameof(volume), "Volume must be a number between 0 and 1");
            if (note < 0) throw new ArgumentOutOfRangeException(nameof(note), "Note cannot be negative");
            return GetInvoker()(note, volume);
        }
    }
}