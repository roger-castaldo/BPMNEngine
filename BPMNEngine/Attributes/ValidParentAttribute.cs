namespace BPMNEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true,Inherited =true)]
    internal class ValidParentAttribute : Attribute
    {
        public Type Parent { get; private init; }

        public ValidParentAttribute(Type parent)
        {
            Parent = parent;
        }
    }
}
