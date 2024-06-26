using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTagAttribute("exts","andCondition")]
    internal record AndCondition : ANegatableConditionSet
    {
        public AndCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override bool EvaluateCondition(IReadonlyVariables variables)
            => !Conditions.Any(cond => !cond.Evaluate(variables));

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (Children.Length < 2 )
            {
                err = (err?? []).Append("Not enough child elements found for an And Condition");
                return false;
            }
            return res;
        }
    }
}
