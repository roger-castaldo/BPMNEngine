using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Tasks
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ServiceTask)]
    internal class ServiceTask : AIcon
    {
        private static readonly IIconPart[] _PARTS = [new Cog()]; 
        protected override IIconPart[] Parts => _PARTS;
    }
}
