using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.StartEvent)]
    internal class StartEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[]
        {
            new OuterCircle()
        };

        protected override IIconPart[] Parts { get { return _PARTS; } }
    }
}
