using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    internal abstract record ANegatableCondition : ACondition
    {
        protected bool Negated => (this["negated"]!=null &&bool.Parse(this["negated"]));

        protected abstract ValueTask<bool> EvaluateConditionAsync(IReadonlyVariables variables, IElement owningElement);

        protected ANegatableCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        { }

        public sealed async override ValueTask<bool> IsElementStartValid(IReadonlyVariables variables, IElement owningElement)
            => (Negated ? !(await EvaluateConditionAsync(variables, owningElement)) : await EvaluateConditionAsync(variables, owningElement));

    }
}
