using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ErrorEndEvent)]
    internal class ErrorEndEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new ThickCircle(),
            new Bolt(true)
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
