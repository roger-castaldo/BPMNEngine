using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Conditions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes
{
    [XMLTag("bpmn","sequenceFlow")]
    [RequiredAttribute("id")]
    internal class SequenceFlow : AElement
    {
        public string name { get { return _GetAttributeValue("name"); } }
        public string sourceRef { get { return _GetAttributeValue("sourceRef"); } }
        public string targetRef { get { return _GetAttributeValue("targetRef"); } }

        public SequenceFlow(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public bool IsFlowValid(IsFlowValid isFlowValid,ProcessVariablesContainer variables)
        {
            if (ExtensionElement != null)
            {
                ExtensionElements ee = (ExtensionElements)ExtensionElement;
                if (ee.Children != null)
                {
                    foreach (IElement ie in ee.Children)
                    {
                        if (ie is ConditionSet)
                        {
                            if (!((ConditionSet)ie).Evaluate(variables))
                                return false;
                        }
                    }
                }
            }
            return isFlowValid(this, variables);
        }
    }
}
