using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Collaborations
{
    [XMLTagAttribute("bpmn", "participant")]
    [RequiredAttributeAttribute("processRef")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(Collaboration))]
    internal record Participant : AElement
    {
        public Participant(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
    }
}
