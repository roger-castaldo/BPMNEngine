using BPMNEngine.Elements.Diagrams;

namespace BPMNEngine.Drawing.Icons
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal class IconTypeAttribute(BPMIcons icon) : Attribute
    {
        public BPMIcons Icon { get; private init; } = icon;
    }
}
