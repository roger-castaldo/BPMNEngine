using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;
using Microsoft.Maui.Graphics;

namespace BPMNEngine.Elements.Diagrams
{
    [XMLTagAttribute("di", "waypoint")]
    [RequiredAttributeAttribute("x")]
    [AttributeRegexAttribute("x", "^-?\\d+(\\.\\d+)?$")]
    [RequiredAttributeAttribute("y")]
    [AttributeRegexAttribute("y", "^-?\\d+(\\.\\d+)?$")]
    [ValidParent(typeof(Edge))]
    internal record Waypoint : AElement
    {
        public Waypoint(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
        public PointF Point => new(
                    float.Parse(this["x"]),
                    float.Parse(this["y"])
            );
    }
}
