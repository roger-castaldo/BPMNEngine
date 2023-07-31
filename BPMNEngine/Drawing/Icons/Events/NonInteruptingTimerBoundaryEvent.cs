using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.NonInteruptingTimerBoundaryEvent)]
    internal class NonInteruptingTimerBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(true),
            new InnerCircle(true),
            new Clock()
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
