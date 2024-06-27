using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Tasks
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ScriptTask)]
    internal class ScriptTask : AIcon
    {
        private static readonly IIconPart[] _PARTS = [new Script()];
        protected override IIconPart[] Parts => _PARTS;
    }
}
