using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.NonInteruptingMessageBoundaryEvent)]
    internal class NonInteruptingMessageBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(true),
            new InnerCircle(true),
            new Envelope(false,false)
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
