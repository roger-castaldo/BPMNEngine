using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Collaborations
{
    [XMLTag("bpmn", "text")]
    [ValidParent(typeof(TextAnnotation))]
    internal class Text : AElement
    {
        public string Value =>  Element.InnerText;
        public Text(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }
    }
}
