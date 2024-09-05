using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.InteruptingMessageBoundaryEvent)]
    internal class InteruptingMessageBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new OuterCircle(),
            new InnerCircle(),
            new Envelope(false,false)
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
