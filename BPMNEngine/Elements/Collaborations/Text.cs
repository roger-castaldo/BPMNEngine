using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Collaborations
{
    [XMLTagAttribute("bpmn", "text")]
    [ValidParent(typeof(TextAnnotation))]
    internal record Text : AElement
    {
        public Text(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
        public string Value => Element.InnerText;
    }
}
