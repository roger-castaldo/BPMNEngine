using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.SignalStartEvent)]
    internal class SignalStartEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new OuterCircle(),
            new Triangle(false)
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
