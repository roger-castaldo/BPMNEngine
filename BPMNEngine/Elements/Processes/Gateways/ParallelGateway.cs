using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTagAttribute("bpmn", "parallelGateway")]
    internal record ParallelGateway : AGateway
    {
        public ParallelGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override ValueTask<IEnumerable<string>> EvaulateOutgoingPathsAsync(Definition definition, IsFlowValid isFlowValid, IReadonlyVariables variables)
            => ValueTask.FromResult(Outgoing);
    }
}
