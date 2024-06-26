using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTagAttribute("bpmn","inclusiveGateway")]
    internal record InclusiveGateway : AGateway
    {
        public InclusiveGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
