using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions.Extensions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "signalEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class SignalEventDefinition : AParentElement
    {
        public SignalEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }

        public string[] SignalTypes
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (IElement elem in Children)
                {
                    if (elem is SignalDefinition)
                    {
                        ret.Add((((SignalDefinition)elem).Type == null ? "*" : ((SignalDefinition)elem).Type));
                    }
                }
                if (ret.Count==0 && ExtensionElement!=null)
                {
                    foreach (IElement elem in ((IParentElement)ExtensionElement).Children)
                    {
                        if (elem is SignalDefinition)
                        {
                            ret.Add((((SignalDefinition)elem).Type == null ? "*" : ((SignalDefinition)elem).Type));
                        }
                    }
                }
                return ret.ToArray();
            }
        }

        public override bool IsValid(out string[] err)
        {
            if (SignalTypes.Length != 0)
            {
                List<string> errors = new List<string>();
                IElement[] elems = Definition.LocateElementsOfType((Parent is IntermediateThrowEvent ? typeof(IntermediateCatchEvent) : typeof(IntermediateThrowEvent)));
                if (Parent is IntermediateThrowEvent)
                {
                    if (SignalTypes.Length > 1)
                        errors.Add("A throw event can only have one error to be thrown.");
                    bool found = false;
                    foreach (IntermediateCatchEvent catcher in elems)
                    {
                        foreach (IElement child in catcher.Children)
                        {
                            if (child is SignalEventDefinition)
                            {
                                if (new List<string>(((SignalEventDefinition)child).SignalTypes).Contains(SignalTypes[0]))
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
                                if (child is SignalEventDefinition)
                                {
                                    if (new List<string>(((SignalEventDefinition)child).SignalTypes).Contains("*"))
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
                        errors.Add("A defined signal type needs to have a Catch Event with a corresponding type or all");
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
