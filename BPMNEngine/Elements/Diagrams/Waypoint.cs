using Microsoft.Maui.Graphics;
using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Diagrams
{
    [XMLTag("di","waypoint")]
    [RequiredAttribute("x")]
    [AttributeRegex("x", "^-?\\d+(\\.\\d+)?$")]
    [RequiredAttribute("y")]
    [AttributeRegex("y", "^-?\\d+(\\.\\d+)?$")]
    [ValidParent(typeof(Edge))]
    internal class Waypoint : AElement
    {
        public PointF Point => new(
                    float.Parse(this["x"]),
                    float.Parse(this["y"]));
        public Waypoint(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
