using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes
{
    [XMLTag("bpmn","laneSet")]
    [ValidParent(typeof(IProcess))]
    internal class LaneSet : AParentElement
    {
        public LaneSet(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
