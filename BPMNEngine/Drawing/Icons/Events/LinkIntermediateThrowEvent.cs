using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.LinkIntermediateThrowEvent)]
    internal class LinkIntermediateThrowEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new InnerCircle(),
            new RightArrow(true)
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
