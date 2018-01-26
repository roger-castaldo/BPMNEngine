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

        public new string[] Outgoing
        {
            get
            {
                List<string> ret = new List<string>();
                IElement[] catchers = null;
                foreach (IElement child in Children)
                {
                    if (child is ErrorEventDefinition)
                    {
                        if (((ErrorEventDefinition)child).ErrorTypes.Length == 1)
                        {
                            string errorType = ((ErrorEventDefinition)child).ErrorTypes[0];
                            if (catchers==null)
                                catchers = Definition.LocateElementsOfType(typeof(IntermediateCatchEvent));
                            foreach (IntermediateCatchEvent catcher in catchers)
                            {
                                string[] tmp = catcher.ErrorTypes;
                                if (tmp != null)
                                {
                                    if (new List<string>(tmp).Contains(errorType))
                                        ret.Add(catcher.id);
                                }
                            }
                            if (ret.Count == 0)
                            {
                                foreach (IntermediateCatchEvent catcher in catchers)
                                {
                                    string[] tmp = catcher.ErrorTypes;
                                    if (tmp != null)
                                    {
                                        if (new List<string>(tmp).Contains("*"))
                                            ret.Add(catcher.id);
                                    }
                                }
                            }
                        }
                    }else if (child is SignalEventDefinition)
                    {
                        if (((SignalEventDefinition)child).SignalTypes.Length == 1)
                        {
                            string signalType = ((SignalEventDefinition)child).SignalTypes[0];
                            if (catchers == null)
                                catchers = Definition.LocateElementsOfType(typeof(IntermediateCatchEvent));
                            foreach (IntermediateCatchEvent catcher in catchers)
                            {
                                string[] tmp = catcher.SignalTypes;
                                if (tmp != null)
                                {
                                    if (new List<string>(tmp).Contains(signalType))
                                        ret.Add(catcher.id);
                                }
                            }
                            if (ret.Count == 0)
                            {
                                foreach (IntermediateCatchEvent catcher in catchers)
                                {
                                    string[] tmp = catcher.SignalTypes;
                                    if (tmp != null)
                                    {
                                        if (new List<string>(tmp).Contains("*"))
                                            ret.Add(catcher.id);
                                    }
                                }
                            }
                        }
                    }else if (child is MessageEventDefinition)
                    {
                        if (((MessageEventDefinition)child).MessageTypes.Length == 1)
                        {
                            string messageType = ((MessageEventDefinition)child).MessageTypes[0];
                            if (catchers == null)
                                catchers = Definition.LocateElementsOfType(typeof(IntermediateCatchEvent));
                            foreach (IntermediateCatchEvent catcher in catchers)
                            {
                                string[] tmp = catcher.MessageTypes;
                                if (tmp != null)
                                {
                                    if (new List<string>(tmp).Contains(messageType))
                                        ret.Add(catcher.id);
                                }
                            }
                            if (ret.Count == 0)
                            {
                                foreach (IntermediateCatchEvent catcher in catchers)
                                {
                                    string[] tmp = catcher.MessageTypes;
                                    if (tmp != null)
                                    {
                                        if (new List<string>(tmp).Contains("*"))
                                            ret.Add(catcher.id);
                                    }
                                }
                            }
                        }
                    }
                }
                return (ret.Count == 0 ? base.Outgoing : ret.ToArray());
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
            bool checkOutgoing = true;
            foreach (IElement child in Children)
            {
                if (child is ErrorEventDefinition)
                {
                    checkOutgoing = ((ErrorEventDefinition)child).ErrorTypes.Length==0;
                    break;
                }
                else if (child is SignalEventDefinition)
                {
                    checkOutgoing = ((SignalEventDefinition)child).SignalTypes.Length==0;
                    break;
                }
                else if (child is MessageEventDefinition)
                {
                    checkOutgoing = ((MessageEventDefinition)child).MessageTypes.Length==0;
                    break;
                }
            }
            if (checkOutgoing)
            {
                if (Outgoing == null)
                {
                    err = new string[] { "Intermediate Throw Events must have an outgoing path." };
                    return false;
                }
                else if (Outgoing.Length != 1)
                {
                    err = new string[] { "Intermediate Throw Events must have only 1 outgoing path." };
                    return false;
                }
            }
            return base.IsValid(out err);
        }
    }
}
