using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.IntermediateThrowEvent)]
    internal class IntermediateThrowEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new OuterCircle(),
            new InnerCircle()
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
