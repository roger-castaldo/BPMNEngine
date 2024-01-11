namespace BPMNEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class,Inherited=false,AllowMultiple=false)]
    internal class XMLTag : Attribute
    {
        public string Name { get; private init; }
        public string Prefix { get; private init; }

        public XMLTag(string prefix,string name)
        {
            Name = name;
            Prefix = prefix;
        }

        public override string ToString()
        {
            return (Prefix==null ? Name : $"{Prefix}:{Name}");
        }

        internal bool Matches(XmlPrefixMap map, string tagName)
        {
            return tagName.Equals(ToString(),StringComparison.CurrentCultureIgnoreCase)
                ||tagName.Equals(Name,StringComparison.CurrentCultureIgnoreCase)
                ||(Prefix != null && map.IsMatch(Prefix, Name, tagName));
        }
    }
}
