using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTag("bpmn","complexGateway")]
    internal class ComplexGateway : AGateway
    {
        public ComplexGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override IEnumerable<string> EvaulateOutgoingPaths(Definition definition, IsFlowValid isFlowValid, IReadonlyVariables variables)
        {
            var result = Outgoing
                .Where(o => ((SequenceFlow)definition.LocateElement(o)).IsFlowValid(isFlowValid, variables));
            if (!result.Any() && Default!=null)
                result = new[] { Default };
            return result;
        }
    }
}
