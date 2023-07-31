using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "compensateEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class CompensationEventDefinition : AElement
    {
        public CompensationEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }
    }
}
