using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn","lane")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(LaneSet))]
    internal record Lane: AParentElement
    {
        public Lane(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
        public IEnumerable<string> Nodes 
            => Children.OfType<FlowNodeRef>().Select(elem => elem.Value);
    }
}
