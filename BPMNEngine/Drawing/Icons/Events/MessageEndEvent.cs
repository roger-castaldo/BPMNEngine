using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.MessageEndEvent)]
    internal class MessageEndEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new ThickCircle(),
            new Envelope(true,false)
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
