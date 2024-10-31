using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTagAttribute("bpmn", "parallelGateway")]
    internal record ParallelGateway : AGateway
    {
        public ParallelGateway(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public override ValueTask<IEnumerable<string>> EvaulateOutgoingPathsAsync(Definition definition, IsFlowValid isFlowValid, IReadonlyVariables variables, ILogger logger)
            => ValueTask.FromResult(Outgoing);
    }
}
