using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Collaborations
{
    [XMLTagAttribute("bpmn", "text")]
    [ValidParent(typeof(TextAnnotation))]
    internal record Text : AElement
    {
        public Text(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
        public string Value => Element.InnerText;
    }
}
