using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ConditionalStartEvent)]
    internal class ConditionalStartEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new Note()
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
