using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Tasks
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ReceiveTask)]
    internal class ReceiveTask : AIcon
    {
        private static readonly IIconPart[] _PARTS = [new Envelope(false, true)]; 
        protected override IIconPart[] Parts => _PARTS;
    }
}
