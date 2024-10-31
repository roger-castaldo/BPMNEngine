using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "incoming")]
    [ValidParent(typeof(AFlowNode))]
    internal record IncomingFlow : AElement
    {
        public IncomingFlow(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
        public string Value => Element.InnerText;
    }
}
