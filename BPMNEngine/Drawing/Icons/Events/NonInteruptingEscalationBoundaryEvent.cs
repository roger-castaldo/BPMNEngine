using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.NonInteruptingEscalationBoundaryEvent)]
    internal class NonInteruptingEscalationBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new OuterCircle(true),
            new InnerCircle(true),
            new UpArrow(false)
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
