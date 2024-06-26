using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.EndEvent)]
    internal class EndEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS =
        [
            new ThickCircle()
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
