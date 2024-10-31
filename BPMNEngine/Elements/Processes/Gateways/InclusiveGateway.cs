using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTagAttribute("bpmn", "inclusiveGateway")]
    internal record InclusiveGateway : AGateway
    {
        public InclusiveGateway(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
    }
}
