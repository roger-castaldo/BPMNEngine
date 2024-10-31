using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "outgoing")]
    [ValidParent(typeof(AFlowNode))]
    internal record OutgoingFlow : AElement
    {
        public OutgoingFlow(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
        public string Value => Element.InnerText;
    }
}
