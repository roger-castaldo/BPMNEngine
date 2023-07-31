using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Gateways
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ParallelGateway)]
    internal class ParallelGateway : AGateway
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new CenterPlus()
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
