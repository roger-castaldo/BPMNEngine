using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.SignalIntermediateCatchEvent)]
    internal class SignalIntermediateCatchEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new OuterCircle(),
            new InnerCircle(),
            new Triangle(false)
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
