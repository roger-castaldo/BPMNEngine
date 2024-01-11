using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes
{
    [XMLTag("bpmn", "extensionElements")]
    [ValidParent(null)]
    internal class ExtensionElements : AParentElement
    {
        public ExtensionElements(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

    }
}
