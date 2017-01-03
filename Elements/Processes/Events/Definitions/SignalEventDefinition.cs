using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "signalEventDefinition")]
    internal class SignalEventDefinition : AElement
    {
        public SignalEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }
    }
}
