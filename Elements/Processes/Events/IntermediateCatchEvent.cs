using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    [XMLTag("bpmn", "intermediateCatchEvent")]
    internal class IntermediateCatchEvent : AEvent
    {
        public IntermediateCatchEvent(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }
    }
}
