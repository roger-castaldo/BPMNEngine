using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.SignalIntermediateThrowEvent)]
    internal class SignalIntermediateThrowEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new OuterCircle(),
            new InnerCircle(),
            new Triangle(true)
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
