using EasyJSON;
using RemoteOS.OpenComputers.Data;
using RemoteOS.Helpers;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RemoteOS.OpenComputers
{
    /// <summary>
    /// Base class for OpenComputers components
    /// </summary>
    public abstract class Component
    {
        int? _slot;
        string? _type;
        string? _handle;

        // To prevent race condition
        TaskCompletionSource<string>? handle_lock;

        public Component(Machine parent, Guid address)
        {
            Parent = parent;
            Address = address;
        }

        public Machine Parent { get; private set; }
        public Guid Address { get; private set; }
        public string Type => _type ??= GetType().GetCustomAttribute<ComponentAttribute>()!.Codename; // If the component has been instantiated then it must have the attribute

        /// <returns>The tier of this component</returns>
        public virtual Task<Tier> GetTier() => Task.FromResult(GetType().GetCustomAttribute<TierAttribute>()?.Tier ?? Tier.None);

        /// <returns>The handle for this component. e.g. component.robot0</returns>
        public async Task<string> GetHandle()
        {
            if (_handle is not null) return _handle;
            if (handle_lock is not null) return await handle_lock.Task;
            handle_lock = new();
			var count = 0;
			var components = await Parent.GetComponents();
			while (components.Any(x => x._handle == $"component.{Type}{count}")) count++;
			await Parent.RawExecute(@$"component.{Type}{count} = component.proxy(""{Address}"")");
			_handle = $"component.{Type}{count}";
            handle_lock.SetResult(_handle);
			return _handle;
        }

        /// <summary>
        /// Invokes the mothod on this component
        /// </summary>
        /// <param name="methodName">The method to invoke</param>
        /// <param name="args">Arguments to call the method with</param>
        /// <returns>Result of the method invocation</returns>
        public async Task<JSONNode> Invoke(string methodName, params object[] args) => await Parent.Execute($"{await GetHandle()}.{methodName}({string.Join(",", args.Select(x => x.Luaify()))})");

		/// <summary>
		/// Invokes the mothod on this component and returns the first element from the result
		/// </summary>
		/// <param name="methodName">The method to invoke</param>
		/// <param name="args">Arguments to call the method with</param>
		/// <returns>Result of the method invocation</returns>
		public async Task<JSONNode> InvokeFirst(string methodName, params object[] args) => (await Invoke(methodName, args))[0];
        
        /// <returns>The slot this component is occupying</returns>
        public async Task<int> GetSlot() => _slot ??= await InvokeFirst("slot");

#if ROS_PROPERTIES
        public int Slot => GetSlot().Result;
#endif
    }
}
