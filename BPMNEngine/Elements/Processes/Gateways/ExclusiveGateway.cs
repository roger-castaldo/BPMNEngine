using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTagAttribute("bpmn", "exclusiveGateway")]
    internal record ExclusiveGateway : ASinglePathGateway
    {
        public ExclusiveGateway(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
    }
}
