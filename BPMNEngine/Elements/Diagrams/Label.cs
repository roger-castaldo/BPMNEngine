using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Diagrams
{
    [XMLTagAttribute("bpmndi","BPMNLabel")]
    [ValidParent(typeof(Edge))]
    [ValidParent(typeof(Shape))]
    internal record Label : AParentElement
    {
        public Label(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
        public Bounds Bounds
            => Children.OfType<Bounds>().FirstOrDefault();
    }
}
