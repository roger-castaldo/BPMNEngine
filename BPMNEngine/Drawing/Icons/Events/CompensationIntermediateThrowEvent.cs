using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.CompensationIntermediateThrowEvent)]
    internal class CompensationIntermediateThrowEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new InnerCircle(),
            new Rewind(true)
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
