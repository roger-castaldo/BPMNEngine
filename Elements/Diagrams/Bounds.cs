using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    [XMLTag("dc","Bounds")]
    internal class Bounds : AElement
    {
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(
                    (int)double.Parse(_GetAttributeValue("x")),
                    (int)double.Parse(_GetAttributeValue("y")),
                    (int)double.Parse(_GetAttributeValue("width")),
                    (int)double.Parse(_GetAttributeValue("height")));
            }
        }

        public Bounds(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

    }
}
