using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.TimerStartEvent)]
    internal class TimerStartEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new Clock()
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
