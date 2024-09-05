using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Gateways
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ParallelGateway)]
    internal class ParallelGateway : AGateway
    {
        private static readonly IIconPart[] _PARTS = [
            new CenterPlus()
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
