using Microsoft.Maui.Graphics;
using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Diagrams
{
    [XMLTagAttribute("di","waypoint")]
    [RequiredAttributeAttribute("x")]
    [AttributeRegexAttribute("x", "^-?\\d+(\\.\\d+)?$")]
    [RequiredAttributeAttribute("y")]
    [AttributeRegexAttribute("y", "^-?\\d+(\\.\\d+)?$")]
    [ValidParent(typeof(Edge))]
    internal record Waypoint: AElement
    {
        public Waypoint(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
        public PointF Point => new(
                    float.Parse(this["x"]),
                    float.Parse(this["y"])
            );
    }
}
