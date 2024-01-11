using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "greaterThanOrEqualCondition")]
    internal class GreaterThanOrEqualCondition : ACompareCondition
    {
        public GreaterThanOrEqualCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override bool EvaluateCondition(IReadonlyVariables variables)
            => Compare(variables) >= 0;
    }
}
