using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "lane")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(LaneSet))]
    internal record Lane : AParentElement
    {
        public Lane(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
        public IEnumerable<string> Nodes
            => Children.OfType<FlowNodeRef>().Select(elem => elem.Value);
    }
}
