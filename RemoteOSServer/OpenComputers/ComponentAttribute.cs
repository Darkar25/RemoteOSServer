namespace RemoteOS.OpenComputers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute {
        public string Codename;
        public ComponentAttribute(string codename) => Codename = codename;
    }
}
