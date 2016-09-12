using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Tasks
{
    [XMLTag("bpmn", "sendTask")]
    internal class SendTask : ATask
    {
        public SendTask(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }
    }
}
