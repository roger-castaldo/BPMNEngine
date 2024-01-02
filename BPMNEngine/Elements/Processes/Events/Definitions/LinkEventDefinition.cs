using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "linkEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class LinkEventDefinition : AElement, IEventDefinition
    {
        public LinkEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public EventSubTypes Type => EventSubTypes.Link;
    }
}
