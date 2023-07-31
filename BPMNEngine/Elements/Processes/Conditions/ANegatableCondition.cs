using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    internal abstract class ANegatableCondition : ACondition
    {
        protected bool Negated => (this["negated"]!=null &&bool.Parse(this["negated"]));

        protected abstract bool EvaluateCondition(IReadonlyVariables variables);

        public ANegatableCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        { }

        public sealed override bool Evaluate(IReadonlyVariables variables)
            => (Negated ? !EvaluateCondition(variables) : EvaluateCondition(variables));
    }
}
