using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn","laneSet")]
    [ValidParent(typeof(IProcess))]
    internal record LaneSet: AParentElement
    {
        public LaneSet(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }

}
