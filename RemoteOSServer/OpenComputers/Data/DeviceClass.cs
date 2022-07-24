namespace RemoteOS.OpenComputers.Data
{
    public enum DeviceClass
    {
        /// <summary>
        /// used to refer to the whole machine, e.g. "Computer", "Server", "Robot"
        /// </summary>
        System,
        /// <summary>
        /// internal bus converter, maybe useful for some low-level archs?
        /// </summary>
        Bridge,
        /// <summary>
        /// memory bank that can contain data, executable code, e.g. RAM, EEPROM
        /// </summary>
        Memory,
        /// <summary>
        /// execution processor, e.g. CPU, cryptography support
        /// </summary>
        Processor,
        /// <summary>
        /// memory address range, e.g. video memory (again, low-level archs maybe?)
        /// </summary>
        Address,
        /// <summary>
        /// storage controller, e.g. IDE controller (low-level...)
        /// </summary>
        Storage,
        /// <summary>
        /// random-access storage device, e.g. floppies
        /// </summary>
        Disk,
        /// <summary>
        /// sequential-access storage device, e.g. cassette tapes
        /// </summary>
        Tape,
        /// <summary>
        /// device-connecting bus, e.g. USB
        /// </summary>
        Bus,
        /// <summary>
        /// network interface, e.g. ethernet, wlan
        /// </summary>
        Network,
        /// <summary>
        /// display adapter, e.g. graphics cards
        /// </summary>
        Display,
        /// <summary>
        /// user input device, e.g. keyboard, mouse
        /// </summary>
        Input,
        /// <summary>
        /// printing device, e.g. printer, 3D-printer
        /// </summary>
        Printer,
        /// <summary>
        /// audio/video device, e.g. sound cards
        /// </summary>
        Multimedia,
        /// <summary>
        /// line communication device, e.g. modem, serial ports
        /// </summary>
        Communication,
        /// <summary>
        /// energy source, e.g. battery, power supply
        /// </summary>
        Power,
        /// <summary>
        /// disk volume, e.g. file system
        /// </summary>
        Volume,
        /// <summary>
        /// generic device (used when no other class is suitable)
        /// </summary>
        Generic
    }
}
