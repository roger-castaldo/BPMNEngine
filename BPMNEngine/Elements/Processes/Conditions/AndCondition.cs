using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTag("exts","andCondition")]
    internal class AndCondition : ANegatableConditionSet
    {
        public AndCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override bool EvaluateCondition(IReadonlyVariables variables)
            => !Conditions.Any(cond => !cond.Evaluate(variables));

        public override bool IsValid(out string[] err)
        {
            if (Children.Count() < 2 )
            {
                err = new string[] { "Not enough child elements found" };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
