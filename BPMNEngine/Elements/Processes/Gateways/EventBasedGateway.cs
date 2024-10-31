using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTagAttribute("bpmn", "eventBasedGateway")]
    internal record EventBasedGateway : ASinglePathGateway
    {
        public EventBasedGateway(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
    }
}
