using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "extensionElements")]
    [ValidParent(null)]
    internal record ExtensionElements: AParentElement
    {
        public ExtensionElements(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
