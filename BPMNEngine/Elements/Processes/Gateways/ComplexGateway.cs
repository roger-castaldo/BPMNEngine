using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTagAttribute("bpmn", "complexGateway")]
    internal record ComplexGateway : AGateway
    {
        public ComplexGateway(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
    }
}
