using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTag("bpmn","inclusiveGateway")]
    internal class InclusiveGateway : AGateway
    {
        public InclusiveGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
