using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTagAttribute("exts", "ConditionSet")]
    [ValidParent(typeof(ExtensionElements))]
    internal record ConditionSet : AConditionSet
    {
        public ConditionSet(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public async override ValueTask<bool> IsElementStartValidAsync(IReadonlyVariables variables, IElement owningElement, ILogger logger)
        {
            try
            {
                return await Conditions.First().IsElementStartValidAsync(variables, owningElement, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error checking if Element start is Valid");
                return false;
            }
        }

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            if (Children.Length > 1)
            {
                errors = errors.Append("Too many children found.");
                isValid=false;
            }
            return (isValid, errors);
        }
    }
}
