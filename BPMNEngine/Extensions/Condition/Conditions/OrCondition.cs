using BPMNEngine.Attributes;
using BPMNEngine.Extensions.Conditions;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Extensions.Condition.Conditions
{
    [XMLTag("exts", "orCondition")]
    internal record OrCondition : ANegatableCondition
    {
        public OrCondition(XmlElement xmlElement, IBaseElement? parent, IExtensionElementFactory elementFactory)
            : base(xmlElement, parent, elementFactory)
        {
        }

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            var isValid = true;
            IEnumerable<string> errors = [];
            Children.OfType<IValidatableElement>().ForEach(child =>
            {
                (var childValid, var childErrors) = child.IsValid(logger);
                isValid &= childValid;
                errors = errors.Concat(childErrors);
            });
            if (Children.OfType<IStepElementStartCheckExtensionElement>().Count() < 2)
            {
                errors = errors.Append("Not enough child elements found for an Or Condition");
                isValid = false;
            }
            return (isValid, errors);
        }
        protected override ValueTask<bool> EvaluateConditionAsync(IReadonlyVariables variables, IElement owningElement, ILogger? logger)
            => Children.OfType<IStepElementStartCheckExtensionElement>().AnyAsync(cond => cond.IsElementStartValidAsync(variables, owningElement, logger));
    }
}
