using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "terminateEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class TerminateEventDefinition : AParentElement, IEventDefinition
    {
        public TerminateEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public EventSubTypes Type => EventSubTypes.Terminate;
    }
}
