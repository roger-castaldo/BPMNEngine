using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "incoming")]
    [ValidParent(typeof(AFlowNode))]
    internal record IncomingFlow : AElement
    {
        public IncomingFlow(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
        public string Value => Element.InnerText;
    }
}
