using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Tasks
{
    [XMLTag("bpmn", "userTask")]
    internal class UserTask : ATask
    {
        public UserTask(XmlElement elem)
            : base(elem) { }
    }
}
