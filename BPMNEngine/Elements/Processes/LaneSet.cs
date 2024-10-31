using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "laneSet")]
    [ValidParent(typeof(IProcess))]
    internal record LaneSet : AParentElement
    {
        public LaneSet(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
    }

}
