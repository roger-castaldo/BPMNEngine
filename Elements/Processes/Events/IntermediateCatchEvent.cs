using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    [XMLTag("bpmn", "intermediateCatchEvent")]
    internal class IntermediateCatchEvent : AEvent
    {
        public IntermediateCatchEvent(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem, map,parent) { }

        public string[] ErrorTypes
        {
            get
            {
                foreach (IElement child in Children)
                {
                    if (child is ErrorEventDefinition)
                        return ((ErrorEventDefinition)child).ErrorTypes;
                }
                return null;
            }
        }

        public string[] MessageTypes
        {
            get
            {
                foreach (IElement child in Children)
                {
                    if (child is MessageEventDefinition)
                        return ((MessageEventDefinition)child).MessageTypes;
                }
                return null;
            }
        }

        public string[] SignalTypes
        {
            get
            {
                foreach (IElement child in Children)
                {
                    if (child is SignalEventDefinition)
                        return ((SignalEventDefinition)child).SignalTypes;
                }
                return null;
            }
        }

        public override bool IsValid(out string[] err)
        {
            bool checkIncoming = true;
            foreach (IElement child in Children)
            {
                if (child is ErrorEventDefinition)
                {
                    checkIncoming = ((ErrorEventDefinition)child).ErrorTypes.Length == 0;
                    break;
                }else if (child is SignalEventDefinition)
                {
                    checkIncoming = ((SignalEventDefinition)child).SignalTypes.Length == 0;
                    break;
                }
                else if (child is MessageEventDefinition)
                {
                    checkIncoming = ((MessageEventDefinition)child).MessageTypes.Length == 0;
                    break;
                }
            }
            if (checkIncoming)
            {
                if ((Incoming == null ? new string[0] : Incoming).Length == 0)
                {
                    err = new string[] { "Intermediate Catch Events must have an incoming path." };
                    return false;
                }
            }
            if (Outgoing == null)
            {
                err = new string[] { "Intermediate Catch Events must have an outgoing path." };
                return false;
            }else if (Outgoing.Length != 1)
            {
                err = new string[] { "Intermediate Catch Events must have only 1 outgoing path." };
                return false;
            }
            return base.IsValid(out err);
        }
        
    }
}
