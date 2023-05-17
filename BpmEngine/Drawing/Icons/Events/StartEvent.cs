using BpmEngine.Drawing.Icons.IconParts;

namespace BpmEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.StartEvent)]
    internal class StartEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[]
        {
            new OuterCircle()
        };

        protected override IIconPart[] _parts { get { return _PARTS; } }
    }
}
