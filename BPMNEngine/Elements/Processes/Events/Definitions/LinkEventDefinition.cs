using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "linkEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class LinkEventDefinition : AElement
    {
        public LinkEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }
    }
}
