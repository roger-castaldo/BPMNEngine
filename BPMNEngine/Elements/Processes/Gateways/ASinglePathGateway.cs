using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Gateways
{
    internal abstract record ASinglePathGateway : AGateway
    {
        protected ASinglePathGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override sealed async ValueTask<IEnumerable<string>> EvaulateOutgoingPathsAsync(Definition definition, IsFlowValid isFlowValid, IReadonlyVariables variables)
        {
            var result = await base.EvaulateOutgoingPathsAsync(definition, isFlowValid, variables);
            if (result.Count()>1)
                throw new MultipleOutgoingPathsException(this);
            return result;
        }
    }
}
