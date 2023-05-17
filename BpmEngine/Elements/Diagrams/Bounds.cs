using Microsoft.Maui.Graphics;
using BpmEngine.Attributes;

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Diagrams
{
    [XMLTag("dc","Bounds")]
    [RequiredAttribute("x")]
    [AttributeRegex("x","^-?\\d+(\\.\\d+)?$")]
    [RequiredAttribute("y")]
    [AttributeRegex("y", "^-?\\d+(\\.\\d+)?$")]
    [RequiredAttribute("width")]
    [AttributeRegex("width", "^\\d+(\\.\\d+)?$")]
    [RequiredAttribute("height")]
    [AttributeRegex("height", "^\\d+(\\.\\d+)?$")]
    [ValidParent(typeof(Label))]
    [ValidParent(typeof(Shape))]
    internal class Bounds : AElement
    {
        public RectF Rectangle => new RectF(
                    float.Parse(this["x"]),
                    float.Parse(this["y"]),
                    float.Parse(this["width"]),
                    float.Parse(this["height"]));

        public Bounds(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

    }
}
