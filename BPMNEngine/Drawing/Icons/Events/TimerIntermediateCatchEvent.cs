using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.TimerIntermediateCatchEvent)]
    internal class TimerIntermediateCatchEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new InnerCircle(),
            new Clock()
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
