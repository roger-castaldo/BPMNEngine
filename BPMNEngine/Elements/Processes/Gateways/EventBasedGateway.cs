using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTagAttribute("bpmn", "eventBasedGateway")]
    internal record EventBasedGateway : ASinglePathGateway
    {
        public EventBasedGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
