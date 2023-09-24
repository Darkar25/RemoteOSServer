namespace RemoteOS.Helpers
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ComponentAttribute : Attribute
	{
		public string Codename { get; init; }
		public ComponentAttribute(string codename) => Codename = codename;
	}
}