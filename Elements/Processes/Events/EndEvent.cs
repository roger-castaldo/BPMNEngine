using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Org.Reddragonit.BpmEngine.Interfaces;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    [XMLTag("bpmn","endEvent")]
    internal class EndEvent : AEvent
    {
        public EndEvent(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public bool IsProcessEnd
        {
            get
            {
                foreach(IElement child in Children)
                {
                    if (child is CompensationEventDefinition || child is ConditionalEventDefinition || child is ErrorEventDefinition || child is EscalationEventDefinition || child is LinkEventDefinition || child is SignalEventDefinition || child is MessageEventDefinition || child is TimerEventDefinition)
                        return false;
                }
                return true;
            }
        }

        public override bool IsValid(out string[] err)
        {
            if (Outgoing != null)
            {
                if (Outgoing.Length > 0)
                {
                    err = new string[] { "End Events cannot have an outgoing path." };
                    return false;
                }
            }
            if (Incoming == null)
            {
                err = new string[] { "End Events must have an incoming path." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
