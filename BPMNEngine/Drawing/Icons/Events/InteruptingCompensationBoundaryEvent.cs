using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.InteruptingCompensationBoundaryEvent)]
    internal class InteruptingCompensationBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new OuterCircle(),
            new InnerCircle(),
            new Rewind(false)
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
