using BPMNEngine.Elements.Processes.Scripts;

namespace BPMNEngine.Elements.Processes.Conditions
{
    internal abstract record AConditionSet : ACondition
    {
        protected AConditionSet(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected IEnumerable<ACondition> Conditions
            => Children
            .OfType<ACondition>().Concat(
                Children
                .OfType<AScript>()
                .Select(asc => new ScriptCondition(asc))
            );

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            if (!Children.Any())
            {
                errors = errors.Append("No child elements found within a condition set.");
                isValid = false;
            }
            return (isValid, errors);
        }
    }
}
