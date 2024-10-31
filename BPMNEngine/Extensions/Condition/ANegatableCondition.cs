using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Extensions.Conditions
{
    internal abstract record ANegatableCondition : ACondition
    {
        protected ANegatableCondition(XmlElement xmlElement, IBaseElement? parent, IExtensionElementFactory elementFactory) 
            : base(xmlElement, parent, elementFactory){}

        private bool Negated => bool.TryParse(this["negated"], out var b) && b;
        protected abstract ValueTask<bool> EvaluateConditionAsync(IReadonlyVariables variables, IElement owningElement, ILogger? logger);
        public sealed async override ValueTask<bool> IsElementStartValidAsync(IReadonlyVariables variables, IElement owningElement, ILogger? logger)
            => (Negated ? !(await EvaluateConditionAsync(variables, owningElement, logger)) : await EvaluateConditionAsync(variables, owningElement, logger));
    }
}
