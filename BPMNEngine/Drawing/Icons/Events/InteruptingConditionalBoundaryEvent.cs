using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.InteruptingConditionalBoundaryEvent)]
    internal class InteruptingConditionalBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new InnerCircle(),
            new Note()
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
