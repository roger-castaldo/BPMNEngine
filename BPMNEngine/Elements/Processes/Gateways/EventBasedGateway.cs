using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTag("bpmn","eventBasedGateway")]
    internal class EventBasedGateway : ASinglePathGateway
    {
        public EventBasedGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
