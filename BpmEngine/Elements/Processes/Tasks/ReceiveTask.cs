using BpmEngine.Attributes;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Processes.Tasks
{
    [XMLTag("bpmn", "receiveTask")]
    internal class ReceiveTask : ATask
    {
        public ReceiveTask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
