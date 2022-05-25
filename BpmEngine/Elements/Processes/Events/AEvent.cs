using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    [ValidParent(typeof(IProcess))]
    internal abstract class AEvent : AFlowNode
    {
        public EventSubTypes? SubType
        {
            get
            {
                EventSubTypes? ret = null;
                foreach (IElement ie in this.Children)
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
                    }
                }
                return ret;
            }
        }

        public AEvent(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public TimeSpan? GetTimeout(IReadonlyVariables variables)
        {
            foreach (IElement ie in Children)
            {
                if (ie is TimerEventDefinition)
                    return ((TimerEventDefinition)ie).GetTimeout(variables);
            }
            return null;
        }
    }
}
