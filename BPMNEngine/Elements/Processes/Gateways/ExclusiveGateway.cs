using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTagAttribute("bpmn","exclusiveGateway")]
    internal record ExclusiveGateway : ASinglePathGateway
    {
        public ExclusiveGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
