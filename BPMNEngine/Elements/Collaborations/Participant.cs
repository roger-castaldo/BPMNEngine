using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Collaborations
{
    [XMLTag("bpmn", "participant")]
    [RequiredAttribute("processRef")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Collaboration))]
    internal class Participant : AElement
    {
        public Participant(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
