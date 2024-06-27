namespace BPMNEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal class XMLTagAttribute(string prefix, string name) : Attribute
    {
        public string Name { get; private init; } = name;
        public string Prefix { get; private init; } = prefix;

        public override string ToString()
            => (Prefix==null ? Name : $"{Prefix}:{Name}");

        internal bool Matches(XmlPrefixMap map, string tagName)
            => tagName.Equals(ToString(), StringComparison.CurrentCultureIgnoreCase)
                    ||tagName.Equals(Name, StringComparison.CurrentCultureIgnoreCase)
                    ||(Prefix != null && map.IsMatch(Prefix, Name, tagName));
    }
}
