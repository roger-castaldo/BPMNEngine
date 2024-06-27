using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.MessageIntermediateThrowEvent)]
    internal class MessageIntermediateThrowEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new OuterCircle(),
            new InnerCircle(),
            new Envelope(true,false)
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
