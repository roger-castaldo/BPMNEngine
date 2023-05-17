using BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "compensateEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class CompensationEventDefinition : AElement
    {
        public CompensationEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }
    }
}
