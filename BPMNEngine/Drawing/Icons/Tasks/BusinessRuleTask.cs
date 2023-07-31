using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Tasks
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.BusinessRuleTask)]
    internal class BusinessRuleTask : AIcon
    {
        private static readonly IIconPart[] _PARTS = new[] {
            new Table()
        };

        protected override IIconPart[] Parts => _PARTS;
    }
}
