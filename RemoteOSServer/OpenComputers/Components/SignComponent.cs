namespace RemoteOS.OpenComputers.Components
{
    [Component("sign")]
    public class SignComponent : Component
    {
        public SignComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <returns>The text on the sign in front of the host.</returns>
        public async Task<string> GetValue() => (await Invoke("getValue"))[0];
        /// <summary>
        /// Set the text on the sign in front of the host.
        /// </summary>
        /// <param name="value">New value</param>
        public async Task SetValue(string value) => await Invoke("setValue", $@"""{value}""");

#if ROS_PROPERTIES && ROS_PROPS_UNCACHED
        public string Value
        {
            get => GetValue().Result;
            set => SetValue(value);
        }
#endif
    }
}
