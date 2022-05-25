using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes
{
    [XMLTag("bpmn", "flowNodeRef")]
    [ValidParent(typeof(Lane))]
    internal class FlowNodeRef : AElement
    {

        public string Value { get { return Element.InnerText; } }
        public FlowNodeRef(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }
    }
}
