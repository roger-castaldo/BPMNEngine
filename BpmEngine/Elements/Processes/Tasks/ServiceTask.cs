using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Tasks
{
    [XMLTag("bpmn", "serviceTask")]
    internal class ServiceTask : ATask
    {
        public ServiceTask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
