using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTagAttribute("exts", "andCondition")]
    internal record AndCondition : ANegatableConditionSet
    {
        public AndCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override ValueTask<bool> EvaluateConditionAsync(IReadonlyVariables variables, IElement owningElement, ILogger logger)
            => Conditions.AllAsync(cond => cond.IsElementStartValidAsync(variables, owningElement, logger));

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            if (Children.Length < 2)
            {
                errors = errors.Append("Not enough child elements found for an And Condition");
                isValid=false;
            }
            return (isValid, errors);
        }
    }
}
