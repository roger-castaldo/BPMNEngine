using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.CompensationEndEvent)]
    internal class CompensationEndEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new ThickCircle(),
            new Rewind(true)
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
