﻿using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    [XMLTag("bpmn", "intermediateThrowEvent")]
    internal class IntermediateThrowEvent : AEvent
    {
        public IntermediateThrowEvent(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem, map,parent) { }

        public string Message
        {
            get
            {
                if (SubType.HasValue)
                {
                    switch(SubType.Value)
                    {
                        case EventSubTypes.Signal:
                            return Children
                                .Where(child => child is SignalEventDefinition)
                                .Select(child => ((SignalEventDefinition)child).SignalTypes.First())
                                .FirstOrDefault();
                        case EventSubTypes.Error:
                            return Children
                                .Where(child => child is ErrorEventDefinition)
                                .Select(child => ((ErrorEventDefinition)child).ErrorTypes.First())
                                .FirstOrDefault();
                        case EventSubTypes.Message:
                            return Children
                                .Where(child => child is MessageEventDefinition)
                                .Select(child => ((MessageEventDefinition)child).MessageTypes.First())
                                .FirstOrDefault();
                    }
                }
                return null;
            }
        }

        public override bool IsValid(out string[] err)
        {
            if (!Incoming.Any())
            {
                err = new string[] { "Intermediate Throw Events must have an incoming path." };
                return false;
            }else if (Incoming.Count()!= 1)
            {
                err = new string[] { "Intermediate Throw Events must have only 1 incoming path." };
                return false;
            }
            if (SubType.HasValue)
            {
                switch (SubType.Value)
                {
                    case EventSubTypes.Signal:
                        if (!Children.Any(child=>child is SignalEventDefinition && ((SignalEventDefinition)child).SignalTypes.Any()))
                        {
                            err = new string[] { "Intermediate Throw Signal Events must specify a singal type." };
                            return false;
                        }else if (Children.Any(child => child is SignalEventDefinition && ((SignalEventDefinition)child).SignalTypes.Any() && ((SignalEventDefinition)child).SignalTypes.Count()!=1))
                        {
                            err = new string[] { "Intermediate Throw Signal Events can only specify a single singal." };
                            return false;
                        }
                        break;
                    case EventSubTypes.Error:
                        if (!Children.Any(child => child is ErrorEventDefinition && ((ErrorEventDefinition)child).ErrorTypes.Any()))
                        {
                            err = new string[] { "Intermediate Throw Error Events must specify an error." };
                            return false;
                        }
                        else if (Children.Any(child => child is ErrorEventDefinition && ((ErrorEventDefinition)child).ErrorTypes.Any() && ((ErrorEventDefinition)child).ErrorTypes.Count()!=1))
                        {
                            err = new string[] { "Intermediate Throw Error Events can only specify a single error." };
                            return false;
                        }
                        break;
                    case EventSubTypes.Message:
                        if (!Children.Any(child => child is MessageEventDefinition && ((MessageEventDefinition)child).MessageTypes.Any()))
                        {
                            err = new string[] { "Intermediate Throw Message Events must specify a message." };
                            return false;
                        }
                        else if (Children.Any(child => child is MessageEventDefinition && ((MessageEventDefinition)child).MessageTypes.Any() && ((MessageEventDefinition)child).MessageTypes.Count()!=1))
                        {
                            err = new string[] { "Intermediate Throw Message Events can only specify a single message." };
                            return false;
                        }
                        break;
                }
            }
            return base.IsValid(out err);
        }
    }
}
