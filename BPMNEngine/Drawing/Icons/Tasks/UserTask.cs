using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Tasks
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.UserTask)]
    internal class UserTask : AIcon
    {
        private static readonly IIconPart[] _PARTS = new[] { new Person() }; 
        protected override IIconPart[] Parts => _PARTS;
    }
}
