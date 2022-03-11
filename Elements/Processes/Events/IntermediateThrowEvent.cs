using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    [XMLTag("bpmn", "intermediateThrowEvent")]
    internal class IntermediateThrowEvent : AEvent
    {
        public IntermediateThrowEvent(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem, map,parent) { }

        private object _message = null;
        public object Message
        {
            get
            {
                if (_message==null && SubType.HasValue)
                {
                    switch (SubType.Value)
                    {
                        case EventSubTypes.Signal:
                            foreach (IElement child in Children)
                            {
                                if (child is SignalEventDefinition)
                                {
                                    _message = ((SignalEventDefinition)child).SignalTypes[0];
                                    break;
                                }
                            }
                            break;
                        case EventSubTypes.Error:
                            foreach (IElement child in Children)
                            {
                                if (child is ErrorEventDefinition)
                                {
                                    _message = new Exception(((ErrorEventDefinition)child).ErrorTypes[0]);
                                    break;
                                }
                            }
                            break;
                        case EventSubTypes.Message:
                            foreach (IElement child in Children)
                            {
                                if (child is MessageEventDefinition)
                                {
                                    _message = ((MessageEventDefinition)child).MessageTypes[0];
                                    break;
                                }
                            }
                            break;
                    }
                }
                return _message;
            }
        }

        public override bool IsValid(out string[] err)
        {
            if (Incoming == null)
            {
                err = new string[] { "Intermediate Throw Events must have an incoming path." };
                return false;
            }else if (Incoming.Length != 1)
            {
                err = new string[] { "Intermediate Throw Events must have only 1 incoming path." };
                return false;
            }
            if (SubType.HasValue)
            {
                bool found = false;
                switch (SubType.Value)
                {
                    case EventSubTypes.Signal:
                        foreach (IElement child in Children)
                        {
                            if (child is SignalEventDefinition)
                            {
                                if (((SignalEventDefinition)child).SignalTypes!=null) {
                                    found=true;
                                    if (((SignalEventDefinition)child).SignalTypes.Length!=1)
                                    {
                                        err = new string[] { "Intermediate Throw Signal Events can only specify a single singal." };
                                        return false;
                                    }
                                }
                                break;
                            }
                        }
                        if (!found)
                        {
                            err = new string[] { "Intermediate Throw Signal Events must specify a singal type." };
                            return false;
                        }
                        break;
                    case EventSubTypes.Error:
                        foreach (IElement child in Children)
                        {
                            if (child is ErrorEventDefinition)
                            {
                                if (((ErrorEventDefinition)child).ErrorTypes!=null)
                                {
                                    found=true;
                                    if (((ErrorEventDefinition)child).ErrorTypes.Length!=1)
                                    {
                                        err = new string[] { "Intermediate Throw Error Events can only specify a single error." };
                                        return false;
                                    }
                                }
                                break;
                            }
                        }
                        if (!found)
                        {
                            err = new string[] { "Intermediate Throw Error Events must specify an error." };
                            return false;
                        }
                        break;
                    case EventSubTypes.Message:
                        foreach (IElement child in Children)
                        {
                            if (child is MessageEventDefinition)
                            {
                                if (((MessageEventDefinition)child).MessageTypes!=null)
                                {
                                    found=true;
                                    if (((MessageEventDefinition)child).MessageTypes.Length!=1)
                                    {
                                        err = new string[] { "Intermediate Throw Message Events can only specify a single message." };
                                        return false;
                                    }
                                }
                                break;
                            }
                        }
                        if (!found)
                        {
                            err = new string[] { "Intermediate Throw Message Events must specify a message." };
                            return false;
                        }
                        break;
                }
            }
            return base.IsValid(out err);
        }
    }
}
