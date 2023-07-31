using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes
{
    [XMLTag("bpmn", "flowNodeRef")]
    [ValidParent(typeof(Lane))]
    internal class FlowNodeRef : AElement
    {
        public string Value => Element.InnerText;
        public FlowNodeRef(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }
    }
}
