namespace BPMNEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal class XMLTagAttribute(string prefix, string name) : Attribute
    {
        public string Name { get; private init; } = name;
        public string Prefix { get; private init; } = prefix;
    }
}
