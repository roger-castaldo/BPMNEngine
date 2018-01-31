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
    [ValidParent(typeof(Process))]
    internal class SequenceFlow : AElement
    {
        public string name { get { return this["Name"]; } }
        public string sourceRef { get { return this["sourceRef"]; } }
        public string targetRef { get { return this["targetRef"]; } }

        public SequenceFlow(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public bool IsFlowValid(IsFlowValid isFlowValid,ProcessVariablesContainer variables)
        {
            Log.Debug("Checking if Sequence Flow[{0}] is valid", new object[] { id });
            bool ret = isFlowValid(this, variables);
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
