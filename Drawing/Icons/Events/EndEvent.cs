using Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.EndEvent)]
    internal class EndEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[]
        {
            new ThickCircle()
        };

        protected override IIconPart[] _parts { get { return _PARTS; } }
    }
}
