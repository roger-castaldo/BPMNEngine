using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    [XMLTag("di","waypoint")]
    [RequiredAttribute("x")]
    [AttributeRegex("x", "^-?\\d+(\\.\\d+)?$")]
    [RequiredAttribute("y")]
    [AttributeRegex("y", "^-?\\d+(\\.\\d+)?$")]
    internal class Waypoint : AElement
    {
        public Point Point
        {
            get
            {
                return new Point(
                    (int)double.Parse(_GetAttributeValue("x")),
                    (int)double.Parse(_GetAttributeValue("y")));
            }
        }

        public Waypoint(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
