using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTagAttribute("bpmn", "escalationEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal record EscalationEventDefinition : AElement, IEventDefinition
    {
        public EventSubTypes Type => EventSubTypes.Escalation;
        public EscalationEventDefinition(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
    }
}
