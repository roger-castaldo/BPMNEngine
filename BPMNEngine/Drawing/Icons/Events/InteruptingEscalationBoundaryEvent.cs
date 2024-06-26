using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.InteruptingEscalationBoundaryEvent)]
    internal class InteruptingEscalationBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new OuterCircle(),
            new InnerCircle(),
            new UpArrow(false)
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
