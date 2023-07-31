using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.MessageIntermediateCatchEvent)]
    internal class MessageIntermediateCatchEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new InnerCircle(),
            new Envelope(false,false)
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
