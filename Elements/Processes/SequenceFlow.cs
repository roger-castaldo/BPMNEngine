using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Conditions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes
{
    [XMLTag("bpmn","sequenceFlow")]
    internal class SequenceFlow : AElement
    {
        public string name { get { return _GetAttributeValue("name"); } }
        public string sourceRef { get { return _GetAttributeValue("sourceRef"); } }
        public string targetRef { get { return _GetAttributeValue("targetRef"); } }

        public SequenceFlow(XmlElement elem)
            : base(elem) { }

        public bool IsFlowValid(IsFlowValid isFlowValid,ProcessVariablesContainer variables)
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
            return isFlowValid(this, variables);
        }
    }
}
