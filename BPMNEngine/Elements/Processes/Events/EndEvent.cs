using BPMNEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using BPMNEngine.Interfaces;
using BPMNEngine.Elements.Processes.Events.Definitions;
using System.Linq;

namespace BPMNEngine.Elements.Processes.Events
{
    [XMLTag("bpmn","endEvent")]
    internal class EndEvent : AEvent
    {
        public EndEvent(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public bool IsProcessEnd => !Children
            .Any(child => child is CompensationEventDefinition || child is ConditionalEventDefinition || child is ErrorEventDefinition || child is EscalationEventDefinition || child is LinkEventDefinition || child is SignalEventDefinition || child is MessageEventDefinition || child is TimerEventDefinition);

        public bool IsTermination => Children
            .Any(child => child is TerminateEventDefinition);

        public override bool IsValid(out string[] err)
        {
            if (Outgoing.Any())
            {
                err = new string[] { "End Events cannot have an outgoing path." };
                return false;
            }
            if (!Incoming.Any())
            {
                err = new string[] { "End Events must have an incoming path." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
