using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;
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

        public async ValueTask<bool> IsFlowValidAsync(IsFlowValid isFlowValid, IReadonlyVariables variables)
        {
            Debug("Checking if Sequence Flow[{0}] is valid", ID);
            return isFlowValid(this, variables)
                && (
                    ExtensionElement==null
                    || (await ExtensionElement.Children.OfType<IStepElementStartCheckExtensionElement>()
                        .AllAsync(check=>check.IsElementStartValid(variables,this)))
                );
        }
    }
}
