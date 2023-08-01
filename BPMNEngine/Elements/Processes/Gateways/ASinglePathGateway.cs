using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Gateways
{
    internal abstract class ASinglePathGateway : AGateway
    {
        public ASinglePathGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override sealed IEnumerable<string> EvaulateOutgoingPaths(Definition definition, IsFlowValid isFlowValid, IReadonlyVariables variables)
        {
            var result = base.EvaulateOutgoingPaths(definition, isFlowValid, variables);
            if (result.Count()>1)
                throw new MultipleOutgoingPathsException(this);
            return result;
        }
    }
}
