using BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Processes
{
    [XMLTag("bpmn", "outgoing")]
    [ValidParent(typeof(AFlowNode))]
    internal class OutgoingFlow : AElement
    {
        public string Value =>Element.InnerText;

        public OutgoingFlow(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }
    }
}
