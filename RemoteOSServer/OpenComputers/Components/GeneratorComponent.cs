namespace RemoteOS.OpenComputers.Components
{
    [Component("generator")]
    public class GeneratorComponent : Component
    {
        public GeneratorComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Tries to insert fuel from the selected slot into the generator's queue.
        /// </summary>
        /// <param name="count">How much items to insert into generator</param>
        /// <returns>Wheter the insertion was successful and the reason why if it wasnt</returns>
        public async Task<(bool Success, string Reason)> Insert(int count = 64)
        {
            var res = await Invoke("insert", count);
            return (res[0], res[1]);
        }
        /// <summary>
        /// Tries to remove items from the generator's queue.
        /// </summary>
        /// <param name="count">How much items to remove</param>
        /// <returns></returns>
        public async Task<bool> Remove(int count = int.MaxValue) => (await Invoke("remove", count))[0];
        /// <returns>The size of the item stack in the generator's queue.</returns>
        public async Task<int> GetCount() => (await Invoke("count"))[0];

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public int Count => GetCount().Result;
#endif
    }
}
