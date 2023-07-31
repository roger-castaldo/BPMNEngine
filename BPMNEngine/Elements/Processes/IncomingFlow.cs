using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes
{
    [XMLTag("bpmn", "incoming")]
    [ValidParent(typeof(AFlowNode))]
    internal class IncomingFlow : AElement
    {
        public string Value => Element.InnerText;

        public IncomingFlow(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }
    }
}
