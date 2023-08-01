using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTag("bpmn","exclusiveGateway")]
    internal class ExclusiveGateway : ASinglePathGateway
    {
        public ExclusiveGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
