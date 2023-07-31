using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes
{
    [XMLTag("bpmn","lane")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(LaneSet))]
    internal class Lane : AParentElement
    {
        public IEnumerable<string> Nodes 
            => Children.OfType<FlowNodeRef>().Select(elem => elem.Value);

        public Lane(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map,parent) { }
    }
}
