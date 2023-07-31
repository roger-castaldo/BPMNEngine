using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.NonInteruptingSignalBoundaryEvent)]
    internal class NonInteruptingSignalBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(true),
            new InnerCircle(true),
            new Triangle(false)
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
