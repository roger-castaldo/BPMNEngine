using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    [XMLTag("di","waypoint")]
    [RequiredAttribute("x")]
    [AttributeRegex("x", "^-?\\d+(\\.\\d+)?$")]
    [RequiredAttribute("y")]
    [AttributeRegex("y", "^-?\\d+(\\.\\d+)?$")]
    [ValidParent(typeof(Edge))]
    internal class Waypoint : AElement
    {
        public Point Point => new Point(
                    float.Parse(this["x"]),
                    float.Parse(this["y"]));
        public Waypoint(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
