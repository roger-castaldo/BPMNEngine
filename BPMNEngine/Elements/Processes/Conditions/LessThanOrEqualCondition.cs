using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTagAttribute("exts", "lessThanOrEqualCondition")]
    internal record LessThanOrEqualCondition : ACompareCondition
    {
        public LessThanOrEqualCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override bool EvaluateCondition(IReadonlyVariables variables)
            => Compare(variables) <= 0;
    }
}
