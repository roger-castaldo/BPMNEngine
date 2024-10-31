using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTagAttribute("bpmn", "conditionalEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal record ConditionalEventDefinition : AElement, IEventDefinition
    {
        public ConditionalEventDefinition(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public EventSubTypes Type => EventSubTypes.Conditional;

        public async ValueTask<bool> IsValidAsync(IReadonlyVariables variables, ILogger? logger)
            => await (
                ExtensionElement?.Extensions.OfType<IStepElementStartCheckExtensionElement>().AnyAsync(check => check.IsElementStartValidAsync(variables, this, logger))
                ??ValueTask.FromResult(false)
            );
    }
}
