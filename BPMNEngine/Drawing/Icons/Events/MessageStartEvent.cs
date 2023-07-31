using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.MessageStartEvent)]
    internal class MessageStartEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new Envelope(false,false)
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
