using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTagAttribute("bpmn", "conditionalEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal record ConditionalEventDefinition : AElement, IEventDefinition
    {
        public ConditionalEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public EventSubTypes Type => EventSubTypes.Conditional;

        public async ValueTask<bool> IsValidAsync(IReadonlyVariables variables)
            => await (
                ExtensionElement?.Children.OfType<IStepElementStartCheckExtensionElement>().AnyAsync(check => check.IsElementStartValid(variables, this))
                ??ValueTask.FromResult(false)
            );
    }
}
