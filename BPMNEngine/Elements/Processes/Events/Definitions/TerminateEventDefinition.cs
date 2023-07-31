using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "terminateEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class TerminateEventDefinition : AParentElement
    {
        public TerminateEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
