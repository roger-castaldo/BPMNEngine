using BPMNEngine.Attributes;
using BPMNEngine.Extensions.Conditions;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Extensions.Condition
{
    [XMLTagAttribute("exts", "ConditionSet")]
    internal record ConditionSet : ACondition
    {
        public ConditionSet(XmlElement xmlElement, IBaseElement? parent, IExtensionElementFactory elementFactory) 
            : base(xmlElement, parent, elementFactory) {}

        public override async ValueTask<bool> IsElementStartValidAsync(IReadonlyVariables variables, IElement owningElement, ILogger? logger)
        {
            try
            {
                return await Children.OfType<IStepElementStartCheckExtensionElement>().First().IsElementStartValidAsync(variables,owningElement, logger);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error checking if Element start is Valid");
                return false;
            }
        }
        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            var isValid = true;
            IEnumerable<string> errors = [];
            if (!Children.OfType<IStepElementStartCheckExtensionElement>().Any())
            {
                errors = errors.Append("No child elements found within a condition set.");
                isValid=false;
            }
            else if (Children.OfType<IStepElementStartCheckExtensionElement>().Count()>1)
            {
                errors = errors.Append("Too many children found.");
                isValid=false;
            }
            Children.OfType<IValidatableElement>().ForEach(child =>
            {
                (var childValid, var childErrors) = child.IsValid(logger);
                isValid &= childValid;
                errors = errors.Concat(childErrors);
            });
            return (isValid, errors);
        }
    }
}
