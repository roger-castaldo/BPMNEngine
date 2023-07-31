using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "lessThanCondition")]
    internal class LessThanCondition : ACompareCondition
    {
        public LessThanCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override bool EvaluateCondition(IReadonlyVariables variables)
            => Compare(variables) < 0;
    }
}
