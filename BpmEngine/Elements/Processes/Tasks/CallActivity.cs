using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Tasks
{
    [XMLTag("bpmn", "callActivity")]
    internal class CallActivity : ATask
    {
        public CallActivity(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {}
    }
}
