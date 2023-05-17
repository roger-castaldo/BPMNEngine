using BpmEngine.Drawing.Icons.IconParts;

namespace BpmEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.TerminateEndEvent)]
    internal class TerminateEndEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[]
        {
            new ThickCircle(),
            new FilledCircle()
        };

        protected override IIconPart[] _parts { get { return _PARTS; } }
    }
}
