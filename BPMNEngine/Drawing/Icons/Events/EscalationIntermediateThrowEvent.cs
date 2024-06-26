using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.EscalationIntermediateThrowEvent)]
    internal class EscalationIntermediateThrowEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new OuterCircle(),
            new InnerCircle(),
            new UpArrow(true)
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
