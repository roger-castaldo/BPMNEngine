using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTagAttribute("bpmn", "compensateEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal record CompensationEventDefinition : AElement, IEventDefinition
    {
        public CompensationEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public EventSubTypes Type => EventSubTypes.Compensation;
    }
}
