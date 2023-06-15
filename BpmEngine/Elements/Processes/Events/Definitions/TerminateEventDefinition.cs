using BPMNEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "terminateEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class TerminateEventDefinition : AParentElement
    {
        public TerminateEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }
    }
}
