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
    [ValidParent(typeof(IProcess))]
    internal class SequenceFlow : AElement,ISequenceFlow
    {
        public string name { get { return this["Name"]; } }
        public string sourceRef { get { return this["sourceRef"]; } }
        public string targetRef { get { return this["targetRef"]; } }

        private string _conditionExpression;
        public string conditionExpression { get { return _conditionExpression; } }

        public SequenceFlow(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) {
            _conditionExpression=null;
            if (elem.Attributes["conditionExpression"]!=null)
            {
                _conditionExpression=elem.Attributes["conditionExpression"].Value;
            }
            else
            {
                foreach (XmlNode xn in elem.ChildNodes)
                {
                    if (xn.NodeType==XmlNodeType.Element) {
                        if (map.isMatch("bpmn", "conditionExpression", xn.Name))
                        {
                            _conditionExpression=(xn.ChildNodes[0] is XmlCDataSection ? ((XmlCDataSection)xn.ChildNodes[0]).InnerText : xn.InnerText);
                            break;
                        }
                    }
                }
            }
        }

        public bool IsFlowValid(IsFlowValid isFlowValid, IReadonlyVariables variables)
        {
            Debug("Checking if Sequence Flow[{0}] is valid", new object[] { id });
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
