using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Gateways
{
    internal abstract record ASinglePathGateway : AGateway
    {
        protected ASinglePathGateway(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public override sealed async ValueTask<IEnumerable<string>> EvaulateOutgoingPathsAsync(Definition definition, IsFlowValid isFlowValid, IReadonlyVariables variables, ILogger? logger)
        {
            var result = await base.EvaulateOutgoingPathsAsync(definition, isFlowValid, variables, logger);
            if (result.Count()>1)
                throw new MultipleOutgoingPathsException(this);
            return result;
        }
    }
}
