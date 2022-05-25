using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Collaborations
{
    [XMLTag("bpmn", "text")]
    [ValidParent(typeof(TextAnnotation))]
    internal class Text : AElement
    {

        public string Value { get { return Element.InnerText; } }
        public Text(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }
    }
}
