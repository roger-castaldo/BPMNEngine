using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    internal abstract record ANegatableConditionSet : AConditionSet
    {
        protected bool Negated => (this["negated"] != null &&bool.Parse(this["negated"]));

        protected abstract ValueTask<bool> EvaluateConditionAsync(IReadonlyVariables variables, IElement owningElement, ILogger logger);

        public ANegatableConditionSet(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public sealed async override ValueTask<bool> IsElementStartValidAsync(IReadonlyVariables variables, IElement owningElement, ILogger logger)
            => (Negated ? !(await EvaluateConditionAsync(variables, owningElement, logger)) : await EvaluateConditionAsync(variables, owningElement, logger));
    }
}
