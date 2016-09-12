using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Tasks
{
    [XMLTag("bpmn","task")]
    internal class Task : ATask
    {
        public Task(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }
    }
}
