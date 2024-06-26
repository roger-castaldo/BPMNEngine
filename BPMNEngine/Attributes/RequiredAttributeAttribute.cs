namespace BPMNEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true,Inherited=true)]
    internal class RequiredAttributeAttribute(string name) : Attribute
    {
        public string Name { get; private init; } = name;
    }
}
