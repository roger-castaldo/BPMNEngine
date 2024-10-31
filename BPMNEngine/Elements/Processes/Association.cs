using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "association")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(IProcess))]
    internal record Association : AFlowElement
    {
        public Association(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
    }
}
