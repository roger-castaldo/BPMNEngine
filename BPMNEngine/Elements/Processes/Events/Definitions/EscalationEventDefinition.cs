using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTagAttribute("bpmn", "escalationEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal record EscalationEventDefinition : AElement, IEventDefinition
    {
        public EventSubTypes Type => EventSubTypes.Escalation;
        public EscalationEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
