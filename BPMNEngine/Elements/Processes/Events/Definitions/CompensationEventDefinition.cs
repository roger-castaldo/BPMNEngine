using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTagAttribute("bpmn", "compensateEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal record CompensationEventDefinition : AElement, IEventDefinition
    {
        public CompensationEventDefinition(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public EventSubTypes Type 
            => EventSubTypes.Compensation;
    }
}
