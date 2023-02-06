﻿using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Conditions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (
                ExtensionElement!=null &&
                ((ExtensionElements)ExtensionElement).Children != null &&
                !((ExtensionElements)ExtensionElement).Children.Any(ie => ie is ConditionSet && !((ConditionSet)ie).Evaluate(variables))
            )
                return false;
            return true;
        }
    }
}
