using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Conditions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System.Linq;
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
        public string sourceRef =>this["sourceRef"];
        public string targetRef =>this["targetRef"];

        private readonly string _conditionExpression;
        public string conditionExpression =>_conditionExpression;

        public SequenceFlow(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) {
            _conditionExpression=null;
            if (elem.Attributes["conditionExpression"]!=null)
                _conditionExpression=elem.Attributes["conditionExpression"].Value;
            else
            {
                foreach (XmlNode xn in elem.ChildNodes)
                {
                    if (xn.NodeType==XmlNodeType.Element && map.isMatch("bpmn", "conditionExpression", xn.Name))
                    {
                        _conditionExpression=(xn.ChildNodes[0] is XmlCDataSection ? ((XmlCDataSection)xn.ChildNodes[0]).InnerText : xn.InnerText);
                        break;
                    }
                }
            }
        }

        public bool IsFlowValid(IsFlowValid isFlowValid, IReadonlyVariables variables)
        {
            Debug("Checking if Sequence Flow[{0}] is valid", new object[] { id });
            if (!isFlowValid(this, variables))
                return false;
            if (ExtensionElement != null)
            {
                return !((ExtensionElements)ExtensionElement).Children
                    .Any(ie=>ie is ConditionSet && !((ConditionSet)ie).Evaluate(variables));
            }
            return true;
        }
    }
}
