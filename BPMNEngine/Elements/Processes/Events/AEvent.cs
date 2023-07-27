using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Events
{
    [ValidParent(typeof(IProcess))]
    internal abstract class AEvent : AFlowNode
    {
        public EventSubTypes? SubType
        {
            get
            {
                EventSubTypes? ret = null;
                Children.ForEach(ie =>
                {
                    switch (ie.GetType().Name)
                    {
                        case "MessageEventDefinition":
                            ret = EventSubTypes.Message;
                            break;
                        case "TimerEventDefinition":
                            ret = EventSubTypes.Timer;
                            break;
                        case "ConditionalEventDefinition":
                            ret = EventSubTypes.Conditional;
                            break;
                        case "SignalEventDefinition":
                            ret = EventSubTypes.Signal;
                            break;
                        case "EscalationEventDefinition":
                            ret = EventSubTypes.Escalation;
                            break;
                        case "LinkEventDefinition":
                            ret = EventSubTypes.Link;
                            break;
                        case "CompensationEventDefinition":
                            ret = EventSubTypes.Compensation;
                            break;
                        case "ErrorEventDefinition":
                            ret = EventSubTypes.Error;
                            break;
                        case "TerminateEventDefinition":
                            ret = EventSubTypes.Terminate;
                            break;
                    }
                });
                return ret;
            }
        }

        public AEvent(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public TimeSpan? GetTimeout(IReadonlyVariables variables)
        {
            return Children
                .OfType<TimerEventDefinition>()
                .Select(ie => ie.GetTimeout(variables))
                .FirstOrDefault();
        }
    }
}
