using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Conditions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    [XMLTag("bpmn","startEvent")]
    internal class StartEvent : AEvent
    {
        public StartEvent(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        internal bool IsEventStartValid(ProcessVariablesContainer variables, IsEventStartValid isEventStartValid)
        {
            if (ExtensionElement != null)
            {
                if (((ExtensionElements)ExtensionElement).IsInternalExtension)
                {
                    ExtensionElements ee = (ExtensionElements)ExtensionElement;
                    if (ee.Children != null)
                    {
                        if (ee.Children[0] is ConditionSet)
                        {
                            return ((ConditionSet)ee.Children[0]).Evaluate(variables);
                        }
                    }
                }
            }
            return isEventStartValid(this, variables);
        }
    }
}
