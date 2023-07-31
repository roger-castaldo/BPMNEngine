using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Collaborations
{
    [XMLTag("bpmn","messageFlow")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Collaboration))]
    internal class MessageFlow : AFlowElement
    {
        public MessageFlow(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem, map,parent) { }
    }
}
