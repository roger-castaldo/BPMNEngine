using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "sequenceFlow")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(IProcess))]
    internal record SequenceFlow : AFlowElement, ISequenceFlow
    {
        private readonly IElementFactory elementFactory;
        public SequenceFlow(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory)
            => this.elementFactory = elementFactory;

        private string? conditionExpession;
        public string? ConditionExpression
            => conditionExpession??= this["conditionExpression"]
                ?? Element.ChildNodes.Cast<XmlElement>()
                .Where(e => elementFactory.IsOfType<ConditionExpression>(e))
                .Select(e => elementFactory.ProduceInstance(e, this))
                .OfType<ConditionExpression>()
                .FirstOrDefault()?.Value;

        public async ValueTask<bool> IsFlowValidAsync(IsFlowValid isFlowValid, IReadonlyVariables variables, ILogger? logger)
        {
            logger?.LogDebug("Checking if Sequence Flow is valid");
            return isFlowValid(this, variables)
                && (
                    ExtensionElement==null
                    || (await ExtensionElement.Extensions.OfType<IStepElementStartCheckExtensionElement>()
                        .AllAsync(check=>check.IsElementStartValidAsync(variables, this, logger)))
                );
        }
    }
}
