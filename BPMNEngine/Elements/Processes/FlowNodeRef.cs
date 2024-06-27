using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "flowNodeRef")]
    [ValidParent(typeof(Lane))]
    internal record FlowNodeRef : AElement
    {
        public FlowNodeRef(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
        public string Value => Element.InnerText;
    }
}
