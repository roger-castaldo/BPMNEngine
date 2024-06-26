using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Tasks
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ManualTask)]
    internal class ManualTask : AIcon
    {
        private static readonly IIconPart[] _PARTS = [
            new Hand()
        ];

        protected override IIconPart[] Parts => _PARTS;
    }
}
