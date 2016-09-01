using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    [XMLTag("bpmn","startEvent")]
    internal class StartEvent : AEvent
    {
        public StartEvent(XmlElement elem)
            : base(elem) { }
    }
}
