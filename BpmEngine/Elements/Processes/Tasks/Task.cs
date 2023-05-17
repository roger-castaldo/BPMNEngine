using BpmEngine.Attributes;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Processes.Tasks
{
    [XMLTag("bpmn","task")]
    internal class Task : ATask
    {
        public Task(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
