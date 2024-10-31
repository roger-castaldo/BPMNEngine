using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Elements.Collaborations
{
    [XMLTagAttribute("bpmn", "messageFlow")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(Collaboration))]
    internal record MessageFlow : AFlowElement
    {
        public MessageFlow(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
    }
}
