using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTagAttribute("bpmn","complexGateway")]
    internal record ComplexGateway : AGateway
    {
        public ComplexGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
