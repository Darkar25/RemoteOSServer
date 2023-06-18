namespace RemoteOS.Helpers
{
	/// <summary>
	/// Make the source generator ignore the method this attribute is applied to
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ComponentIgnoreAttribute : Attribute { }
}