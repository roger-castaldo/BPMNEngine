using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Collaborations
{
    [XMLTagAttribute("bpmn","messageFlow")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(Collaboration))]
    internal record MessageFlow : AFlowElement
    {
        public MessageFlow(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
