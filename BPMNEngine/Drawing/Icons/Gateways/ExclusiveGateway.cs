using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Gateways
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ExclusiveGateway)]
    internal class ExclusiveGateway : AGateway
    {
        private static readonly IIconPart[] _PARTS = [
            new CenterX()
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
