using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Elements.Diagrams
{
    [XMLTagAttribute("bpmndi", "BPMNLabel")]
    [ValidParent(typeof(Edge))]
    [ValidParent(typeof(Shape))]
    internal record Label : AParentElement
    {
        public Label(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
        public Bounds Bounds
            => Children.OfType<Bounds>().FirstOrDefault();
    }
}
