using BPMNEngine.Elements.Diagrams;

namespace BPMNEngine.Drawing.Icons
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal class IconTypeAttribute : Attribute
    {
        public BPMIcons Icon { get; private init; }
        public IconTypeAttribute(BPMIcons icon)
        {
            Icon = icon;
        }
    }
}
