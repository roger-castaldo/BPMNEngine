using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "association")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(IProcess))]
    internal record Association : AFlowElement
    {
        public Association(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
