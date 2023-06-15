using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Interfaces;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes
{
    [XMLTag("bpmn","sequenceFlow")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(IProcess))]
    internal class SequenceFlow : AFlowElement, ISequenceFlow
    {
        private readonly string _conditionExpression;
        public string conditionExpression =>_conditionExpression;

        public SequenceFlow(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) {
            _conditionExpression=null;
            if (elem.Attributes["conditionExpression"]!=null)
                _conditionExpression=elem.Attributes["conditionExpression"].Value;
            else
                _conditionExpression = elem.ChildNodes.Cast<XmlNode>()
                    .Where(xn => xn.NodeType==XmlNodeType.Element && map.isMatch("bpmn", "conditionExpression", xn.Name))
                    .Select(xn => xn.ChildNodes[0] is XmlCDataSection ? ((XmlCDataSection)xn.ChildNodes[0]).InnerText : xn.InnerText)
                    .FirstOrDefault();
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
