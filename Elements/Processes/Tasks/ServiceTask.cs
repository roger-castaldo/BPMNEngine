using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Tasks
{
    [XMLTag("bpmn", "serviceTask")]
    internal class ServiceTask : ATask
    {
        public ServiceTask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
