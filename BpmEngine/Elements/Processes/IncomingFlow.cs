using BPMNEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes
{
    [XMLTag("bpmn", "incoming")]
    [ValidParent(typeof(AFlowNode))]
    internal class IncomingFlow : AElement
    {
        public string Value { get { return this.Element.InnerText; } }

        public IncomingFlow(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }
    }
}
