using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes
{
    [XMLTag("bpmn", "outgoing")]
    [ValidParent(typeof(AFlowNode))]
    internal class OutgoingFlow : AElement
    {
        public string Value => Element.InnerText;

        public OutgoingFlow(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }
    }
}
