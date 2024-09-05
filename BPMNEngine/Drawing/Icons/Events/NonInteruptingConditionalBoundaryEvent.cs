using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.NonInteruptingConditionalBoundaryEvent)]
    internal class NonInteruptingConditionalBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new OuterCircle(true),
            new InnerCircle(true),
            new Note()
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
