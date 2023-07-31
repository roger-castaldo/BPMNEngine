using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "escalationEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class EscalationEventDefinition : AElement
    {
        public EscalationEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }
    }
}
