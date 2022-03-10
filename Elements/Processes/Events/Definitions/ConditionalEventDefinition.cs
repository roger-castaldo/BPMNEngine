using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Conditions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "conditionalEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class ConditionalEventDefinition : AElement
    {
        public ConditionalEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }

        public bool IsValid(IReadonlyVariables variables)
        {
            bool ret = true;
            if (ExtensionElement != null)
            {
                ExtensionElements ee = (ExtensionElements)ExtensionElement;
                if (ee.Children != null)
                {
                    foreach (IElement ie in ee.Children)
                    {
                        if (ie is ConditionSet)
                            ret = ret & ((ConditionSet)ie).Evaluate(variables);
                    }
                }
            }
            return ret;
        }
    }
}
