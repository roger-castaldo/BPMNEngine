using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.LinkIntermediateCatchEvent)]
    internal class LinkIntermediateCatchEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new OuterCircle(),
            new InnerCircle(),
            new RightArrow(false)
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
