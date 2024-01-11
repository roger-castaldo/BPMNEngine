namespace BPMNEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true,Inherited=true)]
    internal class RequiredAttribute : Attribute
    {
        public string Name { get; private init; }

        public RequiredAttribute(string name)
        {
            Name = name;
        }
    }
}
