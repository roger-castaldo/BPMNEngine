using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ConditionalStartEvent)]
    internal class ConditionalStartEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new OuterCircle(),
            new Note()
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
