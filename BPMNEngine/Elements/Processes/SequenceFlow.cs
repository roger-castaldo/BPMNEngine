using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes
{
    [XMLTag("bpmn","sequenceFlow")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(IProcess))]
    internal class SequenceFlow : AFlowElement, ISequenceFlow
    {
        public string ConditionExpression { get; private init; }

        public SequenceFlow(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) {
            ConditionExpression=null;
            if (elem.Attributes["conditionExpression"]!=null)
                ConditionExpression=elem.Attributes["conditionExpression"].Value;
            else
                ConditionExpression = elem.ChildNodes.Cast<XmlNode>()
                    .Where(xn => xn.NodeType==XmlNodeType.Element && map.IsMatch("bpmn", "conditionExpression", xn.Name))
                    .Select(xn => xn.ChildNodes[0] is XmlCDataSection section ? section.InnerText : xn.InnerText)
                    .FirstOrDefault();
        }

        public bool IsFlowValid(IsFlowValid isFlowValid, IReadonlyVariables variables)
        {
            Debug("Checking if Sequence Flow[{0}] is valid", new object[] { ID });
            if (!isFlowValid(this, variables))
                return false;
            if (ExtensionElement != null)
            {
                return !((ExtensionElements)ExtensionElement).Children
                    .Any(ie=>ie is ConditionSet set && !set.Evaluate(variables));
            }
            return true;
        }
    }
}
