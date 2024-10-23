using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTagAttribute("exts", "orCondition")]
    internal record OrCondition : ANegatableConditionSet
    {
        public OrCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override ValueTask<bool> EvaluateConditionAsync(IReadonlyVariables variables, IElement owningElement, ILogger logger)
            => Conditions.AnyAsync(cond => cond.IsElementStartValidAsync(variables, owningElement, logger));

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            if (Children.Length < 2)
            {
                errors = errors.Append("Not enough child elements found for an Or Condition");
                isValid = false;
            }
            return (isValid, errors);
        }
    }
}
