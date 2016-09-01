using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    [XMLTag("bpmn","endEvent")]
    internal class EndEvent : AEvent
    {
        public EndEvent(XmlElement elem)
            : base(elem) { }
    }
}
