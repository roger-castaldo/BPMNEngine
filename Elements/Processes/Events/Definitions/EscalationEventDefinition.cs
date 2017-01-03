using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "escalationEventDefinition")]
    internal class EscalationEventDefinition : AElement
    {
        public EscalationEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }
    }
}
