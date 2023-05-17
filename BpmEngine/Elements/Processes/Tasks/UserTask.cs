using BpmEngine.Attributes;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Processes.Tasks
{
    [XMLTag("bpmn", "userTask")]
    internal class UserTask : ATask
    {
        public UserTask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
