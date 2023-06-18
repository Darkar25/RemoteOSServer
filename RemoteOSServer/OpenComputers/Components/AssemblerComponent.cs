using RemoteOS.Helpers;

namespace RemoteOS.OpenComputers.Components
{
    [Component("assembler")]
    public partial class AssemblerComponent : Component
    {
        public AssemblerComponent(Machine parent, Guid address) : base(parent, address) { }

        /// <summary>
        /// Start assembling, if possible.
        /// </summary>
        /// <returns>Whether assembly was started or not.</returns>
        public partial Task<bool> Start();

        /// <returns>The current state of the assembler, `busy' or `idle', followed by the progress or template validity, respectively.</returns>
        public async Task<(bool CanStart, string Status, double Progress)> GetStatus()
        {
            var res = await Invoke("status");
            var isb = res[1].IsBoolean;
            return (isb ? res[1] : false, res[0], isb ? 0 : res[1]);
        }
    }
}
