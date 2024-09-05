using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.EscalationEndEvent)]
    internal class EscalationEndEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new ThickCircle(),
            new UpArrow(true)
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
