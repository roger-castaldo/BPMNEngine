using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Tasks
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.SendTask)]
    internal class SendTask : AIcon
    {
        private static readonly IIconPart[] _PARTS = [new Envelope(true, true)]; 
        protected override IIconPart[] Parts => _PARTS;
    }
}
