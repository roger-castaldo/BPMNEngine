using Microsoft.Maui.Graphics;
using BpmEngine.Attributes;

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Diagrams
{
    [XMLTag("di","waypoint")]
    [RequiredAttribute("x")]
    [AttributeRegex("x", "^-?\\d+(\\.\\d+)?$")]
    [RequiredAttribute("y")]
    [AttributeRegex("y", "^-?\\d+(\\.\\d+)?$")]
    [ValidParent(typeof(Edge))]
    internal class Waypoint : AElement
    {
        public PointF Point => new PointF(
                    float.Parse(this["x"]),
                    float.Parse(this["y"]));
        public Waypoint(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
