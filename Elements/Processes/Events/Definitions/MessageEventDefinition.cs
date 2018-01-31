using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions.Extensions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "messageEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class MessageEventDefinition : AParentElement
    {
        public MessageEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }

        public string[] MessageTypes
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (IElement elem in Children)
                {
                    if (elem is MessageDefinition)
                    {
                        ret.Add((((MessageDefinition)elem).Name == null ? "*" : ((MessageDefinition)elem).Name));
                    }
                }
                return ret.ToArray();
            }
        }

        public override bool IsValid(out string[] err)
        {
            if (MessageTypes.Length != 0)
            {
                List<string> errors = new List<string>();
                IElement[] elems = Definition.LocateElementsOfType((Parent is IntermediateThrowEvent ? typeof(IntermediateCatchEvent) : typeof(IntermediateThrowEvent)));
                if (Parent is IntermediateThrowEvent)
                {
                    if (MessageTypes.Length > 1)
                        errors.Add("A throw event can only have one error to be thrown.");
                    bool found = false;
                    foreach (IntermediateCatchEvent catcher in elems)
                    {
                        foreach (IElement child in catcher.Children)
                        {
                            if (child is MessageEventDefinition)
                            {
                                if (new List<string>(((MessageEventDefinition)child).MessageTypes).Contains(MessageTypes[0]))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (found)
                            break;
                    }
                    if (!found)
                    {
                        foreach (IntermediateCatchEvent catcher in elems)
                        {
                            foreach (IElement child in catcher.Children)
                            {
                                if (child is MessageEventDefinition)
                                {
                                    if (new List<string>(((MessageEventDefinition)child).MessageTypes).Contains("*"))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                            }
                            if (found)
                                break;
                        }
                    }
                    if (!found)
                        errors.Add("A defined message needs to have a Catch Event with a corresponding type or all");
                }
                if (errors.Count > 0)
                {
                    err = errors.ToArray();
                    return true;
                }
            }
            return base.IsValid(out err);
        }
    }
}
