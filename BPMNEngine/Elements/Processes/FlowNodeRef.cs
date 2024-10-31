using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "flowNodeRef")]
    [ValidParent(typeof(Lane))]
    internal record FlowNodeRef : AElement
    {
        public FlowNodeRef(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
        public string Value => Element.InnerText;
    }
}
