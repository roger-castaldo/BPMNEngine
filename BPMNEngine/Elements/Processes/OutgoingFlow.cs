using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "outgoing")]
    [ValidParent(typeof(AFlowNode))]
    internal record OutgoingFlow: AElement
    {
        public OutgoingFlow(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
        public string Value => Element.InnerText;
    }
}
