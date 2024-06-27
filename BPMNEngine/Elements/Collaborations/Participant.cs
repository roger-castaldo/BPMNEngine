using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Collaborations
{
    [XMLTagAttribute("bpmn", "participant")]
    [RequiredAttributeAttribute("processRef")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(Collaboration))]
    internal record Participant : AElement
    {
        public Participant(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
