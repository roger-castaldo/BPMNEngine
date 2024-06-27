using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "sequenceFlow")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(IProcess))]
    internal record SequenceFlow : AFlowElement, ISequenceFlow
    {
        public SequenceFlow(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        {
            ConditionExpression = elem.Attributes["conditionExpression"]?.Value
                ?? elem.ChildNodes.Cast<XmlNode>()
                    .Where(xn => xn.NodeType==XmlNodeType.Element && map.IsMatch("bpmn", "conditionExpression", xn.Name))
                    .Select(xn => xn.ChildNodes[0] is XmlCDataSection section ? section.InnerText : xn.InnerText)
                    .FirstOrDefault();
        }
        public string ConditionExpression { get; private init; }

        public bool IsFlowValid(IsFlowValid isFlowValid, IReadonlyVariables variables)
        {
            Debug("Checking if Sequence Flow[{0}] is valid", ID);
            return isFlowValid(this, variables)
                && (
                    ExtensionElement==null
                    || ((ExtensionElements)ExtensionElement).Children
                        .OfType<ConditionSet>()
                        .All(cset => cset.Evaluate(variables))
                );
        }
    }
}
