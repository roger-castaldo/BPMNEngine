using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ConditionalIntermediateCatchEvent)]
    internal class ConditionalIntermediateCatchEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new OuterCircle(),
            new InnerCircle(),
            new Note()
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
