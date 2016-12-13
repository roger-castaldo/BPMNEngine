using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    internal abstract class AEvent : AFlowNode
    {
        public EventSubTypes? SubType
        {
            get
            {
                EventSubTypes? ret = null;
                foreach (XmlNode n in SubNodes)
                {
                    if (n.NodeType == XmlNodeType.Element)
                    {
                        switch (n.Name)
                        {
                            case "bpmn:messageEventDefinition":
                                ret = EventSubTypes.Message;
                                break;
                            case "bpmn:timerEventDefinition":
                                ret = EventSubTypes.Timer;
                                break;
                            case "bpmn:conditionalEventDefinition":
                                ret = EventSubTypes.Conditional;
                                break;
                            case "bpmn:signalEventDefinition":
                                ret = EventSubTypes.Signal;
                                break;
                            case "bpmn:escalationEventDefinition":
                                ret = EventSubTypes.Escalation;
                                break;
                            case "bpmn:linkEventDefinition":
                                ret = EventSubTypes.Link;
                                break;
                            case "bpmn:compensationEventDefinition":
                                ret = EventSubTypes.Compensation;
                                break;
                        }
                    }
                }
                return ret;
            }
        }

        public AEvent(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
