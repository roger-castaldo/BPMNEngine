namespace BPMNEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal class ValidParentAttribute(Type parent) : Attribute
    {
        public Type Parent { get; private init; } = parent;
    }
}
