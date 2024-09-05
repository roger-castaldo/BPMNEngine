using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Gateways
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ComplexGateway)]
    internal class ComplexGateway : AGateway
    {
        private static readonly IIconPart[] _PARTS = [
            new CenterStar()
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
